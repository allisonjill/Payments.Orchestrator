using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Application.Interfaces;
using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Domain.Enums;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Commands.ConfirmPayment;

public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, Payment?>
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentGateway _gateway;
    private readonly ILogger<ConfirmPaymentHandler> _logger;

    public ConfirmPaymentHandler(IPaymentRepository repository, IPaymentGateway gateway, ILogger<ConfirmPaymentHandler> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<Payment?> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetAsync(request.PaymentId);
        if (payment == null) return null;

        if (payment.Status == PaymentStatus.Captured)
        {
            _logger.LogInformation("Payment {PaymentId} already captured", request.PaymentId);
            return payment;
        }

        if (payment.Status != PaymentStatus.Initiated)
        {
            throw new InvalidOperationException($"Payment {request.PaymentId} is in state {payment.Status} and cannot be processed.");
        }

        try
        {
            // 1. Validate
            payment.Validate();
            await _repository.SaveAsync(payment);

            // 2. Authorize
            _logger.LogInformation("Initiating gateway charge for {PaymentId}", request.PaymentId);
            var result = await _gateway.ChargeAsync(payment.Amount, payment.Currency, payment.Id);

            if (result.Success)
            {
                payment.Authorize(result.TransactionId!);
                await _repository.SaveAsync(payment);

                // 3. Capture
                payment.Capture();
                _logger.LogInformation("Payment {PaymentId} captured. TransactionId: {TransactionId}", request.PaymentId, result.TransactionId);
            }
            else
            {
                payment.MarkFailed(result.ErrorMessage ?? "Unknown gateway error");
                _logger.LogWarning("Payment {PaymentId} failed. Reason: {Reason}", request.PaymentId, payment.FailureReason);
            }

            await _repository.SaveAsync(payment);
            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error confirming payment {PaymentId}", request.PaymentId);
            throw;
        }
    }
}
