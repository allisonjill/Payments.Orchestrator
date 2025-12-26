using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Api.Infrastructure.Gateways;

public class MockPaymentGateway : IPaymentGateway
{
    public async Task<GatewayResult> ChargeAsync(decimal amount, string currency, Guid paymentId)
    {
        // Simulate IO latency
        await Task.Delay(100);

        // Simple mock logic:
        // - Amounts ending in .00 succeed
        // - Amounts ending in .99 fail (declined)
        // - Everything else succeeds
        
        // This allows testing both paths easily via the Amount input.

        var centAmount = (int)(amount * 100);
        
        if (centAmount % 100 == 99)
        {
            return new GatewayResult(false, null, "Card_Declined");
        }

        return new GatewayResult(true, $"txn_mock_{Guid.NewGuid()}", null);
    }
}
