using Payments.Orchestrator.Api.RainforestConnector.DTOs;

namespace Payments.Orchestrator.Api.RainforestConnector;

public interface IRainforestClient
{
    Task<RainforestCreatePayinConfigResponse> CreatePayinConfigAsync(RainforestCreatePayinConfigRequest request, CancellationToken ct = default);
    Task<RainforestCreateSessionResponse> CreateSessionAsync(RainforestCreateSessionRequest request, CancellationToken ct = default);
}
