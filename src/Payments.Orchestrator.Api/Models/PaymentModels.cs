using Payments.Orchestrator.Api.Domain;

namespace Payments.Orchestrator.Api.Models;

public record CreatePaymentRequest(decimal Amount, string Currency);

public record PaymentResponse(
    Guid Id,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? GatewayTransactionId,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime? ProcessedAt
)
{
    public static PaymentResponse FromDomain(PaymentIntent intent) => new(
        intent.Id,
        intent.Amount,
        intent.Currency,
        intent.Status,
        intent.GatewayTransactionId,
        intent.FailureReason,
        intent.CreatedAt,
        intent.ProcessedAt
    );
}
