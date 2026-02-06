using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Payments.Orchestrator.Api.RainforestConnector;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;
using Xunit;

namespace Payments.Orchestrator.Tests.RainforestConnector;

public class RainforestClientTests
{
    private readonly Mock<IOptions<RainforestOptions>> _mockOptions;
    private readonly Mock<ILogger<RainforestClient>> _mockLogger;

    public RainforestClientTests()
    {
        _mockOptions = new Mock<IOptions<RainforestOptions>>();
        _mockLogger = new Mock<ILogger<RainforestClient>>();
        
        _mockOptions.Setup(o => o.Value).Returns(new RainforestOptions
        {
            BaseUrl = "https://test.api/",
            ApiKey = "test-key"
        });
    }

    [Fact]
    public async Task CreatePayinConfigAsync_ShouldDeserializeResponse_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new RainforestCreatePayinConfigResponse("payin_config_123");
        var jsonResponse = JsonSerializer.Serialize(new { 
            status = "SUCCESS", 
            data = expectedResponse 
        });

        var client = CreateClientWithResponse(HttpStatusCode.OK, jsonResponse);
        var request = new RainforestCreatePayinConfigRequest("mid_123", 100, "USD", "idem-key", null, null);

        // Act
        var result = await client.CreatePayinConfigAsync(request);

        // Assert
        Assert.Equal("payin_config_123", result.PayinConfigId);
    }

    [Fact]
    public async Task CreatePayinConfigAsync_ShouldThrow_WhenApiReturnsErrorStatus()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(new { 
            status = "ERROR", 
            data = new { } 
        });

        var client = CreateClientWithResponse(HttpStatusCode.OK, jsonResponse);
        var request = new RainforestCreatePayinConfigRequest("mid_123", 10, "USD", "Key", null, null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => client.CreatePayinConfigAsync(request));
    }

    private RainforestClient CreateClientWithResponse(HttpStatusCode statusCode, string content)
    {
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        });

        var httpClient = new HttpClient(handler);
        return new RainforestClient(httpClient, _mockOptions.Object, _mockLogger.Object);
    }

    // Simple fake handler for testing
    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
