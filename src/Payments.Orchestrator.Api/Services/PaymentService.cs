using Payments.Orchestrator.Api.Domain;
using Payments.Orchestrator.Api.Interfaces;

namespace Payments.Orchestrator.Api.Services;

public class PaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentGateway _gateway;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository repository,
        IPaymentGateway gateway,
        ILogger<PaymentService> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<Payment> CreatePaymentAsync(decimal amount, string currency)
    {
        var intent = new Payment(amount, currency);
        await _repository.SaveAsync(intent);
        _logger.LogInformation("Created payment intent {PaymentId} for {Amount} {Currency}", intent.Id, amount, currency);
        return intent;
    }

    public async Task<Payment?> GetPaymentAsync(Guid id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<Payment?> ConfirmPaymentAsync(Guid id)
    {
        var intent = await _repository.GetAsync(id);
        if (intent == null) return null;

        // Idempotency/Concurrency check: if already succeeded, just return it
        if (intent.Status == PaymentStatus.Succeeded)
        {
            _logger.LogInformation("Payment {PaymentId} already succeeded", id);
            return intent;
        }

        // Domain validation for transition
        if (intent.Status != PaymentStatus.Created)
        {
            throw new InvalidOperationException($"Payment {id} is in state {intent.Status} and cannot be confirmed.");
        }

        try
        {
            intent.MarkProcessing();
            await _repository.SaveAsync(intent);

            _logger.LogInformation("Initiating gateway charge for {PaymentId}", id);
            var result = await _gateway.ChargeAsync(intent.Amount, intent.Currency, intent.Id);

            if (result.Success)
            {
                intent.MarkSucceeded(result.TransactionId!);
                _logger.LogInformation("Payment {PaymentId} succeeded. TransactionId: {TransactionId}", id, result.TransactionId);
            }
            else
            {
                intent.MarkFailed(result.ErrorMessage ?? "Unknown gateway error");
                _logger.LogWarning("Payment {PaymentId} failed. Reason: {Reason}", id, intent.FailureReason);
            }

            await _repository.SaveAsync(intent);
            return intent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error confirming payment {PaymentId}", id);
            // In a real system, we might mark as 'Error' or 'Unknown' state
            // keeping it simple for now, potentially leaving it in Processing or Created depending on where it failed
            throw; 
        }
    }
}
