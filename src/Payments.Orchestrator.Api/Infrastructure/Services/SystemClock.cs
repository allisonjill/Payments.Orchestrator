using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Api.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
