using HealthChecks.UI.Client;
using HttpMessage.Middleware;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Projects.Application;
using Projects.Domain.Common.Exceptions;
using Projects.Infrastructure;
using Projects.Infrastructure.DataAccess;
using Serilog;

namespace Projects;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddWebAPIServices();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await ApplyMigration();
        }

        app.UseCors("allowAny");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware<ProjectDomainException>>();
        app.UseSerilogRequestLogging();
        app.MapDefaultControllerRoute();
        app.MapControllers();
        app.MapHealthChecks(
            "/hc",
            new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }
        );
        app.MapHealthChecks(
            "/liveness",
            new HealthCheckOptions { Predicate = r => r.Name.Contains("self") }
        );
        app.Run();

        async Task ApplyMigration()
        {
            using var scope = app.Services.CreateScope();
            var initialiser = scope.ServiceProvider.GetRequiredService<SeedData>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
            return;
        }
    }
}
