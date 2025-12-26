using MediatR;
using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(Guid MerchantId, Guid CustomerId, decimal Amount, string Currency) : IRequest<Payment>;
