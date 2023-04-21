using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Extensions
{
    public static class AzureServiceBusSenderExtension
    {
        public static IServiceCollection AddAzureServiceBusSender(this IServiceCollection services, Action<AzureServiceBusSenderOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<IAzureServiceBusSender, AzureServiceBusSender>();
            return services;
        }
    }
}
