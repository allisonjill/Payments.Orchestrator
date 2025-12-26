using Moq;
using Microsoft.Extensions.Logging;
using Payments.Orchestrator.Api.Application.Features.Payments.Commands.ProcessPayment;
using Payments.Orchestrator.Api.Domain.Entities;
using Payments.Orchestrator.Api.Domain.Enums;
using Payments.Orchestrator.Api.Application.Interfaces;

namespace Payments.Orchestrator.Tests.Features.Payments.Commands;

public class ProcessPaymentHandlerTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly Mock<IPaymentGateway> _gatewayMock;
    private readonly Mock<ILogger<ProcessPaymentHandler>> _loggerMock;
    private readonly ProcessPaymentHandler _handler;

    public ProcessPaymentHandlerTests()
    {
        _repoMock = new Mock<IPaymentRepository>();
        _gatewayMock = new Mock<IPaymentGateway>();
        _loggerMock = new Mock<ILogger<ProcessPaymentHandler>>();
        _handler = new ProcessPaymentHandler(_repoMock.Object, _gatewayMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateChargeAndCapture()
    {
        // Arrange
        var command = new ProcessPaymentCommand(100m, "USD");
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "USD", It.IsAny<Guid>()))
            .ReturnsAsync(new GatewayResult(true, "txn_123", null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.Amount);
        Assert.Equal("USD", result.Currency);
        Assert.Equal(PaymentStatus.Captured, result.Status);
        
        _repoMock.Verify(r => r.SaveAsync(It.IsAny<Payment>()), Times.AtLeast(2)); // Initial + Final
    }

    [Fact]
    public async Task Handle_WhenGatewayDeclines_ShouldMarkFailed()
    {
        // Arrange
        var command = new ProcessPaymentCommand(100m, "EUR");
        _gatewayMock.Setup(g => g.ChargeAsync(100m, "EUR", It.IsAny<Guid>()))
            .ReturnsAsync(new GatewayResult(false, null, "Insufficient Funds"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Failed, result.Status);
        Assert.Equal("Insufficient Funds", result.FailureReason);
        
        _repoMock.Verify(r => r.SaveAsync(It.IsAny<Payment>()), Times.AtLeast(2));
    }
}
