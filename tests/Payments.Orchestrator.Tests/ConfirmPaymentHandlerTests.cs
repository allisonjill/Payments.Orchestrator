using Moq;
using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Application.Features.Payments.Commands.ConfirmPayment;
using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Domain.Enums;
using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Tests.Features.Payments.Commands;

public class ConfirmPaymentHandlerTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly Mock<IPaymentGateway> _gatewayMock;
    private readonly Mock<ILogger<ConfirmPaymentHandler>> _loggerMock;
    private readonly ConfirmPaymentHandler _handler;

    public ConfirmPaymentHandlerTests()
    {
        _repoMock = new Mock<IPaymentRepository>();
        _gatewayMock = new Mock<IPaymentGateway>();
        _loggerMock = new Mock<ILogger<ConfirmPaymentHandler>>();
        _handler = new ConfirmPaymentHandler(_repoMock.Object, _gatewayMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenSucceeded_ShouldReturnSuccess()
    {
        // Arrange
        var intent = new Payment(100m, "USD");
        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);
        
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "USD", intent.Id))
            .ReturnsAsync(new GatewayResult(true, "txn_123", null));

        // Act
        var result = await _handler.Handle(new ConfirmPaymentCommand(intent.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Captured, result!.Status);
        Assert.Equal("txn_123", result.GatewayTransactionId);
        
        _repoMock.Verify(r => r.SaveAsync(intent), Times.AtLeast(3));
    }

    [Fact]
    public async Task Handle_WhenGatewayFails_ShouldMarkFailed()
    {
        // Arrange
        var intent = new Payment(100m, "USD");
        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);
        
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "USD", intent.Id))
            .ReturnsAsync(new GatewayResult(false, null, "Insufficient_Funds"));

        // Act
        var result = await _handler.Handle(new ConfirmPaymentCommand(intent.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Failed, result!.Status);
        Assert.Equal("Insufficient_Funds", result.FailureReason);
        
        _repoMock.Verify(r => r.SaveAsync(intent), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WhenAlreadySucceeded_ShouldBeIdempotent()
    {
        // Arrange
        var intent = new Payment(100m, "USD");
        // Transition to Captured manually
        intent.Validate();
        intent.Authorize("txn_existing");
        intent.Capture();

        _repoMock.Setup(r => r.GetAsync(intent.Id)).ReturnsAsync(intent);

        // Act
        var result = await _handler.Handle(new ConfirmPaymentCommand(intent.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Captured, result!.Status);
        Assert.Equal("txn_existing", result.GatewayTransactionId);

        _gatewayMock.Verify(g => g.ChargeAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
}
