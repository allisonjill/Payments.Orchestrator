using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Application.Interfaces;
using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Payment>
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentGateway _gateway;
    private readonly ILogger<ProcessPaymentHandler> _logger;

    public ProcessPaymentHandler(IPaymentRepository repository, IPaymentGateway gateway, ILogger<ProcessPaymentHandler> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<Payment> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // 1. Create & Persist "Initiated" (Received)
        var payment = new Payment(request.MerchantId, request.CustomerId, request.Amount, request.Currency);
        await _repository.SaveAsync(payment);
        _logger.LogInformation("Payment {PaymentId} Initiated for {Amount} {Currency} (Merchant: {MerchantId})", payment.Id, request.Amount, request.Currency, request.MerchantId);

        try
        {
            // 2. Gateway Call
            _logger.LogInformation("Calling Gateway for {PaymentId}", payment.Id);
            var result = await _gateway.ChargeAsync(request.Amount, request.Currency, payment.Id);

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
        }

        // 4. Save Final State
        await _repository.SaveAsync(payment);
        return payment;
    }
}
