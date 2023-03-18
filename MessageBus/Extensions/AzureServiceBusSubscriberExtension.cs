using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
