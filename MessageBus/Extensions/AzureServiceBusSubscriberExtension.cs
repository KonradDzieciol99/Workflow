using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MessageBus.Extensions
{
    public static class AzureServiceBusSubscriberExtension
    {
        public static IServiceCollection AddAzureServiceBusSubscriber(this IServiceCollection services, Action<AzureServiceBusSubscriberOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<AzureServiceBusSubscriber>();
            services.AddHostedService(sp => sp.GetRequiredService<AzureServiceBusSubscriber>());
            return services;
        }
    }
}
