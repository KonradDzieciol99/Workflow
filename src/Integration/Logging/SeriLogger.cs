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

          var builder = configuration.MinimumLevel.Verbose()
                         .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                         .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Debug)
                         .Enrich.WithMachineName()
                         .Enrich.WithAssemblyName()
                         .Filter.ByExcluding("Contains(RequestPath, '/hc') and StatusCode = 200");

          if (!context.Configuration.GetValue("isTest", true))
          {
               var seqUrl = context.Configuration["ConnectionStrings:seq"] ?? throw new ArgumentNullException("ConnectionStrings:seq");
               builder.WriteTo.Seq(seqUrl);
          }
       };
}
