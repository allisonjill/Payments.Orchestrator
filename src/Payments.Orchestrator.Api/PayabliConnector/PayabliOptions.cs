namespace Payments.Orchestrator.Api.PayabliConnector;

public class PayabliOptions
{
    public const string SectionName = "Payabli";

    public string BaseUrl { get; set; } = "https://api-sandbox.payabli.com/";
    public string ApiKey { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
}
