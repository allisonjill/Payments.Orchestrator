using Payments.Orchestrator.Api.Domain;

namespace Payments.Orchestrator.Api.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetAsync(Guid id);
    Task SaveAsync(Payment payment);
}
