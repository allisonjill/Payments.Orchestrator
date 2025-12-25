using Payments.Orchestrator.Api.Domain;

namespace Payments.Orchestrator.Api.Interfaces;

public interface IPaymentRepository
{
    Task<PaymentIntent?> GetAsync(Guid id);
    Task SaveAsync(PaymentIntent payment);
}
