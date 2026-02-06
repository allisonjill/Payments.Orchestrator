using System.Text.Json;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;
using Xunit;

namespace Payments.Orchestrator.Tests.RainforestConnector;

public class RainforestWebhookTests
{
    [Fact]
    public void Deserialize_ShouldParseEnvelopeCorrectly()
    {
        // Arrange
        var json = @"
        {
            ""event_type"": ""payin.succeeded"",
            ""data"": {
                ""payin_id"": ""payin_123"",
                ""amount"": 500
            }
        }";

        // Act
        var envelope = JsonSerializer.Deserialize<RainforestWebhookEnvelope>(json);

        // Assert
        Assert.NotNull(envelope);
        Assert.Equal("payin.succeeded", envelope.EventType);
        
        var payinId = envelope.Data.GetProperty("payin_id").GetString();
        Assert.Equal("payin_123", payinId);
    }
}
