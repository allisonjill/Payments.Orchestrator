namespace Payments.Orchestrator.Api.Domain;

public static class Currency
{
    public const string USD = "USD";
    public const string EUR = "EUR";
    public const string GBP = "GBP";

    public static readonly HashSet<string> SupportedCurrencies = new() { USD, EUR, GBP };

    public static bool IsSupported(string currency) => 
        !string.IsNullOrWhiteSpace(currency) && SupportedCurrencies.Contains(currency.ToUpper());
}
