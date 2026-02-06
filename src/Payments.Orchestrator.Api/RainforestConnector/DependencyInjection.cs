using Payments.Orchestrator.Api.RainforestConnector;

namespace Microsoft.Extensions.DependencyInjection;

public static class RainforestDependencyInjection
{
    public static IServiceCollection AddRainforestConnector(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RainforestOptions>(configuration.GetSection(RainforestOptions.SectionName));

        services.AddHttpClient<IRainforestClient, RainforestClient>();
        
        services.AddScoped<RainforestPayinOrchestrationService>();

        return services;
    }
}
