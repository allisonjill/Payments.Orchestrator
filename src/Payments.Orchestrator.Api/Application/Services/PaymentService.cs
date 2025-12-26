using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Domain.Enums;
using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Api.Application.Services;

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

    // Orchestrates the full synchronous payment flow: Validate -> Persist -> Gateway -> Update
    public async Task<Payment> ProcessPaymentRequestAsync(decimal amount, string currency)
    {
        // 1. Create & Persist "Initiated" (Received)
        var payment = new Payment(amount, currency);
        await _repository.SaveAsync(payment);
        _logger.LogInformation("Payment {PaymentId} Initiated for {Amount} {Currency}", payment.Id, amount, currency);

        try
        {
            // 2. Gateway Call
            _logger.LogInformation("Calling Gateway for {PaymentId}", payment.Id);
            var result = await _gateway.ChargeAsync(amount, currency, payment.Id);

            // 3. Update State
            if (result.Success)
            {
                payment.Validate();
                payment.Authorize(result.TransactionId!);
                payment.Capture();
                _logger.LogInformation("Payment {PaymentId} Captured. Txn: {TransactionId}", payment.Id, result.TransactionId);
            }
            else
            {
                payment.MarkFailed(result.ErrorMessage ?? "Gateway declined");
                _logger.LogWarning("Payment {PaymentId} Failed. Reason: {Reason}", payment.Id, payment.FailureReason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing payment {PaymentId}", payment.Id);
            payment.MarkFailed("System Error");
            // In production, you might want to retry or have a separate error state
        }

        // 4. Save Final State
        await _repository.SaveAsync(payment);
        return payment;
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
