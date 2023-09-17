using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Projects.Application;
using Projects.Infrastructure;
using Projects.Infrastructure.DataAccess;
using Projects.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Projects;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddWebAPIServices();

        builder.Host.UseSerilog(SeriLogger.Configure);

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
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSerilogRequestLogging();
        app.MapDefaultControllerRoute();
        app.MapControllers();
        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
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