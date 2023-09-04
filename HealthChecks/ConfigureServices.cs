using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Chat;

public static class ConfigureServices
{
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services, IConfiguration configuration)
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
