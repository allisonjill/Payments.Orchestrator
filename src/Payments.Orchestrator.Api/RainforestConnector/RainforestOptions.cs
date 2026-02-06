namespace Payments.Orchestrator.Api.RainforestConnector;

public class RainforestOptions
{
    public const string SectionName = "Rainforest";

    public string BaseUrl { get; set; } = "https://api.sandbox.rainforestpay.com/v1/";
    public string ApiKey { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2024-10-16";
    public string? WebhookSigningKey { get; set; }
    public int DefaultSessionTtlSeconds { get; set; } = 3600; // 1 hour
}
