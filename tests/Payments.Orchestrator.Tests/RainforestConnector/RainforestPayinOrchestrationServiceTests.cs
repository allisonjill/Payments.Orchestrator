using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Payments.Orchestrator.Api.RainforestConnector;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;
using Xunit;

namespace Payments.Orchestrator.Tests.RainforestConnector;

public class RainforestPayinOrchestrationServiceTests
{
    private readonly Mock<IRainforestClient> _mockClient;
    private readonly Mock<IOptions<RainforestOptions>> _mockOptions;
    private readonly Mock<ILogger<RainforestPayinOrchestrationService>> _mockLogger;
    private readonly RainforestPayinOrchestrationService _service;

    public RainforestPayinOrchestrationServiceTests()
    {
        _mockClient = new Mock<IRainforestClient>();
        _mockOptions = new Mock<IOptions<RainforestOptions>>();
        _mockLogger = new Mock<ILogger<RainforestPayinOrchestrationService>>();

        _mockOptions.Setup(o => o.Value).Returns(new RainforestOptions
        {
            DefaultSessionTtlSeconds = 3600
        });

        _service = new RainforestPayinOrchestrationService(
            _mockClient.Object,
            _mockOptions.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task CreatePayinSessionAsync_ShouldOrchestrateCallsAndReturnResult()
    {
        // Arrange
        var request = new CreateRainforestPayinSessionRequest(
            MerchantId: "123",
            Amount: 1000,
            Currency: "USD",
            IdempotencyKey: "test-idempotency-key",
            BillingContact: new RainforestBillingContact("user@example.com", null),
            Metadata: new Dictionary<string, string> { { "order_id", "456" } }
        );

        var payinConfigResponse = new RainforestCreatePayinConfigResponse("payin_config_123");
        var sessionResponse = new RainforestCreateSessionResponse("session_key_abc", "session_id_xyz");

        _mockClient
            .Setup(c => c.CreatePayinConfigAsync(It.IsAny<RainforestCreatePayinConfigRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payinConfigResponse);

        _mockClient
            .Setup(c => c.CreateSessionAsync(It.IsAny<RainforestCreateSessionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionResponse);

        // Act
        var result = await _service.CreatePayinSessionAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal("payin_config_123", result.PayinConfigId);
        Assert.Equal("session_key_abc", result.SessionKey);
        Assert.Equal("test-idempotency-key", result.IdempotencyKey);

        _mockClient.Verify(c => c.CreatePayinConfigAsync(It.Is<RainforestCreatePayinConfigRequest>(r => 
            r.MerchantId == "123" &&
            r.Amount == 1000 && 
            r.Currency == "USD" &&
            r.IdempotencyKey == "test-idempotency-key"
        ), It.IsAny<CancellationToken>()), Times.Once);

        _mockClient.Verify(c => c.CreateSessionAsync(It.Is<RainforestCreateSessionRequest>(r => 
            r.Ttl == 3600 &&
            r.Statements[0].Constraints.Merchant.MerchantId == "123"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
