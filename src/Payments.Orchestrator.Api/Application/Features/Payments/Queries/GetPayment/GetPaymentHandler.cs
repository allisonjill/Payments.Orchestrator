using MediatR;
using Payments.Orchestrator.Api.Application.Interfaces;
using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, Payment?>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Payment?> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.PaymentId);
    }
}
