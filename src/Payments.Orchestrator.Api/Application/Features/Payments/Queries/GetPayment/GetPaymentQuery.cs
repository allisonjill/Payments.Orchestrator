using MediatR;
using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Queries.GetPayment;

public record GetPaymentQuery(Guid PaymentId) : IRequest<Payment?>;
