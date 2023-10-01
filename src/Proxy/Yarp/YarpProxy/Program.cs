using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;

namespace YarpProxy;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services
            .AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy") ?? throw new InvalidOperationException("The expected configuration value 'ReverseProxy' is missing."))
            .AddConfigFilter<CustomConfigFilter>();

        var app = builder.Build();

        app.MapReverseProxy(proxyPipeline =>
        {
            proxyPipeline.Use(async (context, next) =>
            {
                var metaData = context.GetReverseProxyFeature().Route.Config.Metadata;

                if (metaData?.TryGetValue("UnsuccessfulResponseStatusCode", out var unsuccessfulResponseStatusCode) ?? false)
                {
                    context.Response.StatusCode = unsuccessfulResponseStatusCode switch
                    {
                        "404" => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError,
                    };
                    return;
                }

                await next();
            });
        });

        app.Run();
    }
}
