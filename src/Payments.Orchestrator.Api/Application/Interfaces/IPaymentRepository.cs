using Payments.Orchestrator.Api.Domain.Entities;

namespace Payments.Orchestrator.Api.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetAsync(Guid id);
    Task SaveAsync(Payment payment);
}
