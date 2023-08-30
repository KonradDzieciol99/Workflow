using MessageBus.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.Extensions;

public static class AzureServiceBusSubscriberExtension
{
    public static IServiceCollection AddAzureServiceBusSubscriber(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<AzureServiceBusSubscriberOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<AzureServiceBusSubscriber>();
        services.AddHostedService(sp => sp.GetRequiredService<AzureServiceBusSubscriber>());
        return services;
    }
}
