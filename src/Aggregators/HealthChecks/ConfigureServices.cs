using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace HealthChecks;

public static class ConfigureServices
{
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        services.AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.SetEvaluationTimeInSeconds(5);
                })
                .AddInMemoryStorage();

        return services;
    }
}
