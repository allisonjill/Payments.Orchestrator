namespace Payments.Orchestrator.Api.Domain;

public class PaymentIntent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Created;
    public string? GatewayTransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }

    // Constructor for creating new intents
    public PaymentIntent(decimal amount, string currency)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        if (!Domain.Currency.IsSupported(currency)) throw new ArgumentException($"Currency '{currency}' is not supported", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpper();
    }

    // State Transitions
    public void MarkProcessing()
    {
        if (Status != PaymentStatus.Created) 
            throw new InvalidOperationException($"Cannot process payment in state {Status}");
        
        Status = PaymentStatus.Processing;
    }

    public void MarkSucceeded(string transactionId)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot succeed payment in state {Status}");

        Status = PaymentStatus.Succeeded;
        GatewayTransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot fail payment in state {Status}");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
    }
}
