using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

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
