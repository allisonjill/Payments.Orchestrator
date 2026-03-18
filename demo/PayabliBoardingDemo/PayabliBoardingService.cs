using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PayabliBoardingDemo
{
    public class PayabliBoardingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _requestToken;

        public PayabliBoardingService(HttpClient httpClient, string requestToken)
        {
            _httpClient = httpClient;
            _requestToken = requestToken;
            _httpClient.BaseAddress = new Uri("https://api-sandbox.payabli.com/");
        }

        public async Task<string> CreateBoardingFlowAsync(object requestPayload, string email)
        {
            // ----------------------------------------------------------------------------------
            // NOTE: In our old Rainforest setup, calling "create payfac" would instantly
            // provision the merchant paypoint. In Payabli, this endpoint Submits a Boarding
            // Application first. The actual paypoint is created AFTER underwriting and approval.
            // ----------------------------------------------------------------------------------

            // Set the required auth header
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("requestToken", _requestToken);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var content = new StringContent(JsonSerializer.Serialize(requestPayload, jsonOptions), Encoding.UTF8, "application/json");

            // 1. Create the Boarding Application
            var createResponse = await _httpClient.PostAsync("api/Boarding/app", content);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();

            // Error handling for non-success responseCode at HTTP level
            if (!createResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create boarding app. HTTP Status: {createResponse.StatusCode}, Details: {createResponseContent}");
            }

            using var responseDoc = JsonDocument.Parse(createResponseContent);
            var root = responseDoc.RootElement;
            
            // Error handling for non-success responseCode at API Payload level
            // Payabli often returns 200 OK but indicates business-logic error in responseCode (e.g., success is "0" or "00" or "1")
            if (root.TryGetProperty("responseCode", out var code) && code.GetString() != "00" && code.GetString() != "0") 
            {
                var msg = root.TryGetProperty("responseMessage", out var m) ? m.GetString() : "Unknown Error";
                throw new Exception($"Payabli API Error [Boarding Application]: Code {code.GetString()} - {msg}\nDetails: {createResponseContent}");
            }

            // Capture the returned appId (responseData)
            var appId = root.GetProperty("responseData").GetString();

            if (string.IsNullOrEmpty(appId))
            {
                throw new Exception("AppId was null or empty in the successful response.");
            }

            // 2. Create the Boarding Link
            var encodedEmail = Uri.EscapeDataString(email);
            var linkResponse = await _httpClient.PutAsync($"api/Boarding/applink/{appId}/{encodedEmail}", null);
            var linkResponseContent = await linkResponse.Content.ReadAsStringAsync();

            if (!linkResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create boarding link. HTTP Status: {linkResponse.StatusCode}, Details: {linkResponseContent}");
            }

            using var linkDoc = JsonDocument.Parse(linkResponseContent);
            var linkRoot = linkDoc.RootElement;

            if (linkRoot.TryGetProperty("responseCode", out var linkCode) && linkCode.GetString() != "00" && linkCode.GetString() != "0")
            {
                var msg = linkRoot.TryGetProperty("responseMessage", out var m) ? m.GetString() : "Unknown Error";
                throw new Exception($"Payabli API Error [Boarding Link]: Code {linkCode.GetString()} - {msg}\nDetails: {linkResponseContent}");
            }

            // Return the URL link generated
            return linkRoot.GetProperty("responseData").GetString();
        }
    }
}
