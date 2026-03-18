using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PayabliBoardingDemo
{
    public class PayabliPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _requestToken;

        public PayabliPaymentService(HttpClient httpClient, string requestToken)
        {
            _httpClient = httpClient;
            _requestToken = requestToken;
            _httpClient.BaseAddress = new Uri("https://api-sandbox.payabli.com/");
        }

        public async Task<string> ProcessPaymentAsync(object paymentPayload)
        {
            // ----------------------------------------------------------------------------------
            // NOTE: In Rainforest, the Payin session / charge creates a distinct intent or object.
            // In Payabli, standard Authorization & Capture flow handles the full transaction against
            // your specific allocated Merchant ID (Paypoint).
            // ----------------------------------------------------------------------------------

            // Set the required auth header
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("requestToken", _requestToken);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var content = new StringContent(JsonSerializer.Serialize(paymentPayload, jsonOptions), Encoding.UTF8, "application/json");

            // Make the payment request
            var paymentResponse = await _httpClient.PostAsync("api/Transaction", content);
            var paymentResponseContent = await paymentResponse.Content.ReadAsStringAsync();

            // Error handling for non-success responseCode at HTTP level
            if (!paymentResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to process payment. HTTP Status: {paymentResponse.StatusCode}, Details: {paymentResponseContent}");
            }

            using var responseDoc = JsonDocument.Parse(paymentResponseContent);
            var root = responseDoc.RootElement;
            
            // Error handling for non-success responseCode at API Payload level
            // Payabli indicates business-logic error in responseCode (e.g., success is "0" or "00" or "1")
            if (root.TryGetProperty("responseCode", out var code) && code.GetString() != "00" && code.GetString() != "0") 
            {
                var msg = root.TryGetProperty("responseMessage", out var m) ? m.GetString() : "Unknown Error";
                throw new Exception($"Payabli API Error [Payment Processing]: Code {code.GetString()} - {msg}\nDetails: {paymentResponseContent}");
            }

            // Capture the returned transactionId
            var responseData = root.GetProperty("responseData");
            var transactionId = responseData.GetProperty("transactionId").GetString();

            if (string.IsNullOrEmpty(transactionId))
            {
                throw new Exception("TransactionId was null or empty in the successful response.");
            }

            return transactionId;
        }

        public async Task<bool> ProcessVoidAsync(string merchantId, string transactionId)
        {
            var voidPayload = new
            {
                merchantId = merchantId,
                transactionType = "void",
                transactionId = transactionId
            };

            return await ExecuteTransactionActionAsync(voidPayload, "Void");
        }

        public async Task<bool> ProcessRefundAsync(string merchantId, string transactionId, decimal amount)
        {
            var refundPayload = new
            {
                merchantId = merchantId,
                transactionType = "refund",
                amount = amount,
                transactionId = transactionId
            };

            return await ExecuteTransactionActionAsync(refundPayload, "Refund");
        }

        private async Task<bool> ExecuteTransactionActionAsync(object payload, string actionName)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("requestToken", _requestToken);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var content = new StringContent(JsonSerializer.Serialize(payload, jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Transaction", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to process {actionName}. HTTP Status: {response.StatusCode}, Details: {responseContent}");
            }

            using var responseDoc = JsonDocument.Parse(responseContent);
            var root = responseDoc.RootElement;
            
            if (root.TryGetProperty("responseCode", out var code) && code.GetString() != "00" && code.GetString() != "0") 
            {
                var msg = root.TryGetProperty("responseMessage", out var m) ? m.GetString() : "Unknown Error";
                throw new Exception($"Payabli API Error [{actionName}]: Code {code.GetString()} - {msg}\nDetails: {responseContent}");
            }

            return true;
        }
    }
}
