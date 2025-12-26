namespace Payments.Orchestrator.Api.Domain.Entities;

using Payments.Orchestrator.Api.Domain.Enums;
using Payments.Orchestrator.Api.Domain.ValueObjects;

public class Payment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid MerchantId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Initiated;
    public string? GatewayTransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }

    public Payment(Guid merchantId, Guid customerId, decimal amount, string currency)
    {
        if (merchantId == Guid.Empty) throw new ArgumentException("MerchantId is required", nameof(merchantId));
        if (customerId == Guid.Empty) throw new ArgumentException("CustomerId is required", nameof(customerId));
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        if (!Domain.ValueObjects.Currency.IsSupported(currency)) throw new ArgumentException($"Currency '{currency}' is not supported", nameof(currency));

        MerchantId = merchantId;
        CustomerId = customerId;
        Amount = amount;
        Currency = currency.ToUpper();
    }

    public void Validate()
    {
        if (Status != PaymentStatus.Initiated)
             throw new InvalidOperationException($"Cannot validate payment in state {Status}");
        
        Status = PaymentStatus.Validated;
    }

    public void Authorize(string transactionId)
    {
        if (Status != PaymentStatus.Validated)
            throw new InvalidOperationException($"Cannot authorize payment in state {Status}");

        Status = PaymentStatus.Authorized;
        GatewayTransactionId = transactionId;
    }

    public void Capture()
    {
        if (Status != PaymentStatus.Authorized)
            throw new InvalidOperationException($"Cannot capture payment in state {Status}");

        Status = PaymentStatus.Captured;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Authorized)
            throw new InvalidOperationException($"Cannot cancel payment in state {Status}");

        Status = PaymentStatus.Cancelled;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        // Can fail from any non-terminal state
        if (Status == PaymentStatus.Captured || Status == PaymentStatus.Cancelled || Status == PaymentStatus.Failed)
            throw new InvalidOperationException($"Cannot fail payment in terminal state {Status}");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
    }
}
