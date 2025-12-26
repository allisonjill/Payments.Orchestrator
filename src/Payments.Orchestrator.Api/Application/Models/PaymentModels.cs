using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Domain.Enums;

namespace Payments.Orchestrator.Api.Application.Models;

public record CreatePaymentRequest(Guid MerchantId, Guid CustomerId, decimal Amount, string Currency);

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
    public static PaymentResponse FromDomain(Payment intent) => new(
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
