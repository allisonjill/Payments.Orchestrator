using System.Collections.Concurrent;
using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Api.Infrastructure.Persistence;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _store = new();

    public Task<Payment?> GetAsync(Guid id)
    {
        _store.TryGetValue(id, out var payment);
        return Task.FromResult(payment);
    }

    public Task SaveAsync(Payment payment)
    {
        _store.AddOrUpdate(payment.Id, payment, (key, oldValue) => payment);
        return Task.CompletedTask;
    }
}
