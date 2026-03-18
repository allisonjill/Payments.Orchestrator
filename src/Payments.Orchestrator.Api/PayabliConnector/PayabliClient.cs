using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payments.Orchestrator.Api.PayabliConnector.DTOs;

namespace Payments.Orchestrator.Api.PayabliConnector;

public class PayabliClient : IPayabliClient
{
    private readonly HttpClient _httpClient;
    private readonly PayabliOptions _options;
    private readonly ILogger<PayabliClient> _logger;

    public PayabliClient(HttpClient httpClient, IOptions<PayabliOptions> options, ILogger<PayabliClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("requestToken", _options.ApiKey);
    }

    public async Task<PayabliBaseResponse> ProcessPaymentAsync(PayabliAuthCaptureRequest request, CancellationToken ct = default)
    {
        request.EntryPoint = _options.MerchantId; // Ensure merchant ID is set from config
        
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var response = await _httpClient.PostAsJsonAsync("MoneyIn/getpaid", request, jsonOptions, ct);
        
        return await HandleResponseAsync(response, "Process Payment", ct);
    }

    public async Task<PayabliBaseResponse> ProcessVoidAsync(PayabliVoidRequest request, CancellationToken ct = default)
    {
        // Voids are GET requests in Payabli v1
        var response = await _httpClient.GetAsync($"MoneyIn/void/{request.TransactionId}", ct);
        return await HandleResponseAsync(response, "Void", ct);
    }

    public async Task<PayabliBaseResponse> ProcessRefundAsync(PayabliRefundRequest request, CancellationToken ct = default)
    {
        // Refunds are GET requests in Payabli v1
        var response = await _httpClient.GetAsync($"MoneyIn/refund/{request.TransactionId}/{request.Amount}", ct);
        return await HandleResponseAsync(response, "Refund", ct);
    }

    public async Task<PayabliAppLinkResponse> GenerateAppLinkAsync(int appId, string email, CancellationToken ct = default)
    {
        // AppLink generation is a PUT request
        var encodedEmail = Uri.EscapeDataString(email);
        var response = await _httpClient.PutAsync($"Boarding/applink/{appId}/{encodedEmail}?sendEmail=false", null, ct);
        
        var responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Payabli HTTP Error: {StatusCode} - {Content}", response.StatusCode, responseContent);
            throw new HttpRequestException($"Payabli API HTTP error: {response.StatusCode} - {responseContent}");
        }

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Deserialize<PayabliAppLinkResponse>(responseContent, jsonOptions);
            
            if (result == null)
            {
                throw new Exception("Payabli API returned empty response for Generate AppLink.");
            }

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Payabli API Business Error [Generate AppLink]: {Code} - {Message}", result.ResponseCode, result.ResponseText);
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Payabli response: {Content}", responseContent);
            throw;
        }
    }

    public async Task<PayabliQueryPaypointsResponse> QueryPaypointsAsync(int orgId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"Query/paypoints/{orgId}?limitRecord=0", ct);
        return await HandleGenericResponseAsync<PayabliQueryPaypointsResponse>(response, "Query Paypoints", ct);
    }

    public async Task<PayabliBoardingAppResponse> CreateBoardingAppAsync(PayabliBoardingAppRequest request, CancellationToken ct = default)
    {
        // Override orgId in request if required, but usually we trust the request body since it has the templateId and stuff
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var response = await _httpClient.PostAsJsonAsync("Boarding/app", request, jsonOptions, ct);
        return await HandleGenericResponseAsync<PayabliBoardingAppResponse>(response, "Create Boarding App", ct);
    }

    public async Task<PayabliReadAppResponse> ReadBoardingAppAsync(int appId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"Boarding/read/{appId}", ct);
        return await HandleGenericResponseAsync<PayabliReadAppResponse>(response, "Read Boarding App", ct);
    }
    private async Task<PayabliBaseResponse> HandleResponseAsync(HttpResponseMessage response, string actionName, CancellationToken ct)
    {
        var responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Payabli HTTP Error: {StatusCode} - {Content}", response.StatusCode, responseContent);
            throw new HttpRequestException($"Payabli API HTTP error: {response.StatusCode} - {responseContent}");
        }

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Deserialize<PayabliBaseResponse>(responseContent, jsonOptions);
            
            if (result == null)
            {
                throw new Exception($"Payabli API returned empty response for {actionName}.");
            }

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Payabli API Business Error [{ActionName}]: {Code} - {Message}", actionName, result.ResponseCode, result.ResponseMessage);
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Payabli response: {Content}", responseContent);
            throw;
        }
    }

    private async Task<T> HandleGenericResponseAsync<T>(HttpResponseMessage response, string actionName, CancellationToken ct) where T : class
    {
        var responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Payabli HTTP Error: {StatusCode} - {Content}", response.StatusCode, responseContent);
            throw new HttpRequestException($"Payabli API HTTP error: {response.StatusCode} - {responseContent}");
        }

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Deserialize<T>(responseContent, jsonOptions);
            
            if (result == null)
            {
                throw new Exception($"Payabli API returned empty response for {actionName}.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Payabli response: {Content}", responseContent);
            throw;
        }
    }
}
