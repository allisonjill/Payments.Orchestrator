namespace Payments.Orchestrator.Api.Interfaces;

public record GatewayResult(bool Success, string? TransactionId, string? ErrorMessage);

public interface IPaymentGateway
{
    Task<GatewayResult> ChargeAsync(decimal amount, string currency, Guid paymentId);
}
