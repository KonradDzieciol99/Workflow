using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using HttpMessage.Middleware;
using API.Aggregator.Domain.Commons.Exceptions;

namespace API.Aggregator;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services.AddAggregatorServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("allowAny");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware<AggregatorDomainException>>();
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
    }
}
