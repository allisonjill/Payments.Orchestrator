namespace Payments.Orchestrator.Api.Application.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}
