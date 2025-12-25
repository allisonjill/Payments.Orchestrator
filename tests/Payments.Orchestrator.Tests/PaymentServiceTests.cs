using Moq;
using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Domain;
using Payments.Orchestrator.Api.Interfaces;
using Payments.Orchestrator.Api.Services;

namespace Payments.Orchestrator.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly Mock<IPaymentGateway> _gatewayMock;
    private readonly Mock<ILogger<PaymentService>> _loggerMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _repoMock = new Mock<IPaymentRepository>();
        _gatewayMock = new Mock<IPaymentGateway>();
        _loggerMock = new Mock<ILogger<PaymentService>>();
        _service = new PaymentService(_repoMock.Object, _gatewayMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreatePayment_ShouldSaveAndReturnIntent()
    {
        // Arrange
        var amount = 100m;
        var currency = "USD";

        // Act
        var result = await _service.CreatePaymentAsync(amount, currency);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(amount, result.Amount);
        Assert.Equal(currency, result.Currency);
        Assert.Equal(PaymentStatus.Created, result.Status);
        
        _repoMock.Verify(r => r.SaveAsync(It.IsAny<PaymentIntent>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmPayment_WhenSucceeded_ShouldReturnSuccess()
    {
        // Arrange
        var intent = new PaymentIntent(100m, "USD");
        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);
        
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "USD", intent.Id))
            .ReturnsAsync(new GatewayResult(true, "txn_123", null));

        // Act
        var result = await _service.ConfirmPaymentAsync(intent.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Succeeded, result!.Status);
        Assert.Equal("txn_123", result.GatewayTransactionId);
        
        _repoMock.Verify(r => r.SaveAsync(intent), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ConfirmPayment_WhenGatewayFails_ShouldMarkFailed()
    {
        // Arrange
        var intent = new PaymentIntent(100m, "USD");
        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);
        
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "USD", intent.Id))
            .ReturnsAsync(new GatewayResult(false, null, "Insufficient_Funds"));

        // Act
        var result = await _service.ConfirmPaymentAsync(intent.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Failed, result!.Status);
        Assert.Equal("Insufficient_Funds", result.FailureReason);
        
        _repoMock.Verify(r => r.SaveAsync(intent), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ConfirmPayment_WhenAlreadySucceeded_ShouldBeIdempotent()
    {
        // Arrange
        var intent = new PaymentIntent(100m, "USD");
        
        // Reflection to force state for test setup if setter is private, 
        // or just use methods if public. Assuming I can get it to Succeeded via internal/methods or helper.
        // Actually PaymentIntent has public methods to transition.
        intent.MarkProcessing();
        intent.MarkSucceeded("txn_existing");

        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);

        // Act
        var result = await _service.ConfirmPaymentAsync(intent.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Succeeded, result!.Status);
        Assert.Equal("txn_existing", result.GatewayTransactionId);

        _gatewayMock.Verify(g => g.ChargeAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
}
