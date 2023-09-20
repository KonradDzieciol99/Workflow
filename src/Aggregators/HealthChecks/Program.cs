using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace HealthChecks;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddHealthChecksServices();

        var app = builder.Build();

        app.UseRouting();
        app.MapDefaultControllerRoute();
        app.MapHealthChecksUI(config =>
        {
            config.UIPath = "/hc-ui";
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        app.Run();
    }
}