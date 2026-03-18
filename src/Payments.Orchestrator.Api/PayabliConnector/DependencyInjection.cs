using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Payments.Orchestrator.Api.PayabliConnector;

public static class DependencyInjection
{
    public static IServiceCollection AddPayabliConnector(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<PayabliOptions>(
            configuration.GetSection(PayabliOptions.SectionName));

        // In a real app we would add Polly retry/circuit breaker policies here
        services.AddHttpClient<IPayabliClient, PayabliClient>();

        return services;
    }
}
