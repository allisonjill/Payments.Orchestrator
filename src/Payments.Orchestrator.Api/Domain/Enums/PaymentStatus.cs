namespace Payments.Orchestrator.Api.Domain.Enums;

public enum PaymentStatus
{
    Initiated,
    Validated,
    Authorized,
    Captured,
    Failed,
    Cancelled
}
