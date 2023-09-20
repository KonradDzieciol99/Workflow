using HealthChecks.UI.Client;
using IdentityDuende;
using IdentityDuende.Infrastructure.DataAccess;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace IdentityDuende;
public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddWebAPIServices(builder.Configuration);

        builder.Host.UseSerilog(SeriLogger.Configure);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            await ApplyMigration();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();
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