using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;
using Photos.Infrastructure.DataAccess;

namespace Photos;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services.AddWebAPIServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await ApplyMigration();
        }

        app.UseRouting();
        app.UseCors("allowAny");
        app.UseAuthentication();
        app.UseAuthorization();
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
