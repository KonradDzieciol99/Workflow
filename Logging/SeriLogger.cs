using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Logging;

public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
       (context, configuration) =>
       {
           var elasticUri = context.Configuration["ConnectionStrings:seq"] ?? throw new ArgumentNullException("ConnectionStrings:seq");

           configuration.MinimumLevel.Verbose()
                        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                        .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Debug)
                        .WriteTo.Seq(elasticUri)
                        .Enrich.WithMachineName()
                        .Enrich.WithAssemblyName()
                        .Filter.ByExcluding("Contains(RequestPath, '/hc') and StatusCode = 200");
       };
}
