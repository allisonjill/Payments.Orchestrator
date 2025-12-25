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
        var payment = await _repository.GetAsync(id);
        if (payment == null) return null;

        if (payment.Status == PaymentStatus.Captured)
        {
            _logger.LogInformation("Payment {PaymentId} already captured", id);
            return payment;
        }

        if (payment.Status != PaymentStatus.Initiated)
        {
            throw new InvalidOperationException($"Payment {id} is in state {payment.Status} and cannot be processed.");
        }

        try
        {
            // 1. Validate
            payment.Validate();
            await _repository.SaveAsync(payment);

            // 2. Authorize
            _logger.LogInformation("Initiating gateway charge for {PaymentId}", id);
            var result = await _gateway.ChargeAsync(payment.Amount, payment.Currency, payment.Id);

            if (result.Success)
            {
                payment.Authorize(result.TransactionId!);
                await _repository.SaveAsync(payment);

                // 3. Capture (Immediate capture for this simplified flow)
                payment.Capture();
                _logger.LogInformation("Payment {PaymentId} captured. TransactionId: {TransactionId}", id, result.TransactionId);
            }
            else
            {
                payment.MarkFailed(result.ErrorMessage ?? "Unknown gateway error");
                _logger.LogWarning("Payment {PaymentId} failed. Reason: {Reason}", id, payment.FailureReason);
            }

            await _repository.SaveAsync(payment);
            return payment;
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
