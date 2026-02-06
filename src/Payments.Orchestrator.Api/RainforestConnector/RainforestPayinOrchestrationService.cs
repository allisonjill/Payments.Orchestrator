using Microsoft.Extensions.Options;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;

namespace Payments.Orchestrator.Api.RainforestConnector;

public class RainforestPayinOrchestrationService
{
    private readonly IRainforestClient _client;
    private readonly RainforestOptions _options;
    private readonly ILogger<RainforestPayinOrchestrationService> _logger;

    public RainforestPayinOrchestrationService(
        IRainforestClient client,
        IOptions<RainforestOptions> options,
        ILogger<RainforestPayinOrchestrationService> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CreateRainforestPayinSessionResult> CreatePayinSessionAsync(CreateRainforestPayinSessionRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating Rainforest payin session for Merchant {MerchantId}, Amount {Amount} {Currency}", 
            req.MerchantId, req.Amount, req.Currency);

        var idempotencyKey = req.IdempotencyKey ?? Guid.NewGuid().ToString();

        // 1. Create Payin Config
        var payinConfigRequest = new RainforestCreatePayinConfigRequest(
            req.MerchantId,
            req.Amount,
            req.Currency,
            idempotencyKey,
            req.BillingContact,
            req.Metadata
        );

        var payinConfigResponse = await _client.CreatePayinConfigAsync(payinConfigRequest, ct);

        // 2. Create Session
        var sessionRequest = new RainforestCreateSessionRequest(
            _options.DefaultSessionTtlSeconds,
            new List<RainforestSessionStatement>
            {
                new RainforestSessionStatement(
                    new List<string> { "group#payment_component" }, // Correct permission group for payment component
                    new RainforestSessionConstraints(
                        new RainforestMerchantConstraint(req.MerchantId)
                    )
                )
            }
        );

        var sessionResponse = await _client.CreateSessionAsync(sessionRequest, ct);

        return new CreateRainforestPayinSessionResult(
            PayinConfigId: payinConfigResponse.PayinConfigId,
            SessionKey: sessionResponse.SessionKey,
            IdempotencyKey: idempotencyKey
        );
    }
}
