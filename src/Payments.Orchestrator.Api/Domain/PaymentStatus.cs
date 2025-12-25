namespace Payments.Orchestrator.Api.Domain;

public enum PaymentStatus
{
    Initiated,
    Validated,
    Authorized,
    Captured,
    Failed,
    Cancelled
}
