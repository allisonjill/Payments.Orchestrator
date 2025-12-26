using MediatR;
using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(Guid PaymentId) : IRequest<Payment?>;
