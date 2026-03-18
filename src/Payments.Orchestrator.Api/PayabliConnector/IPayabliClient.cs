using Payments.Orchestrator.Api.PayabliConnector.DTOs;

namespace Payments.Orchestrator.Api.PayabliConnector;

public interface IPayabliClient
{
    Task<PayabliBaseResponse> ProcessPaymentAsync(PayabliAuthCaptureRequest request, CancellationToken ct = default);
    Task<PayabliBaseResponse> ProcessVoidAsync(PayabliVoidRequest request, CancellationToken ct = default);
    Task<PayabliBaseResponse> ProcessRefundAsync(PayabliRefundRequest request, CancellationToken ct = default);
    Task<PayabliAppLinkResponse> GenerateAppLinkAsync(int appId, string email, CancellationToken ct = default);
    Task<PayabliQueryPaypointsResponse> QueryPaypointsAsync(int orgId, CancellationToken ct = default);
    Task<PayabliBoardingAppResponse> CreateBoardingAppAsync(PayabliBoardingAppRequest request, CancellationToken ct = default);
    Task<PayabliReadAppResponse> ReadBoardingAppAsync(int appId, CancellationToken ct = default);
}
