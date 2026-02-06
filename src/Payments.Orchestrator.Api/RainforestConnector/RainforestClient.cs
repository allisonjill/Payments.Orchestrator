using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;

namespace Payments.Orchestrator.Api.RainforestConnector;

public class RainforestClient : IRainforestClient
{
    private readonly HttpClient _httpClient;
    private readonly RainforestOptions _options;
    private readonly ILogger<RainforestClient> _logger;

    public RainforestClient(HttpClient httpClient, IOptions<RainforestOptions> options, ILogger<RainforestClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
        if (!string.IsNullOrEmpty(_options.ApiVersion))
        {
            _httpClient.DefaultRequestHeaders.Add("Rainforest-Api-Version", _options.ApiVersion);
        }
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PaymentsOrchestrator/1.0");
    }

    public async Task<RainforestCreatePayinConfigResponse> CreatePayinConfigAsync(RainforestCreatePayinConfigRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("payin_configs", request, ct);
        return await HandleResponse<RainforestCreatePayinConfigResponse>(response, ct);
    }

    public async Task<RainforestCreateSessionResponse> CreateSessionAsync(RainforestCreateSessionRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("sessions", request, ct);
        return await HandleResponse<RainforestCreateSessionResponse>(response, ct);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Rainforest API error: {StatusCode} - {Content}", response.StatusCode, content);
            throw new HttpRequestException($"Rainforest API error: {response.StatusCode} - {content}");
        }

        try 
        {
            // Rainforest API responses are wrapped in a standard envelope or flat depending on endpoint.
            // Assumption based on modern API standards and task description: 
            // "All responses follow a wrapper pattern { status, data, errors }."
            
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
             
            // Check for wrapper
            if (root.TryGetProperty("data", out var dataElement))
            {
                 // Check status if available
                 if (root.TryGetProperty("status", out var statusElement))
                 {
                     var status = statusElement.GetString();
                     if (status != "SUCCESS")
                     {
                         // Using generic exception for now, can be specialized
                         throw new Exception($"Rainforest API returned status: {status}");
                     }
                 }
                 
                 return JsonSerializer.Deserialize<T>(dataElement.GetRawText())!;
            }
            
            // Fallback for non-wrapped or direct mapping
             return JsonSerializer.Deserialize<T>(content)!;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Rainforest response: {Content}", content);
            throw;
        }
    }
}
