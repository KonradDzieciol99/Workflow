using MessageBus.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.Extensions;

public static class AzureServiceBusSenderExtension
{
    public static IServiceCollection AddAzureServiceBusSender(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<AzureServiceBusSenderOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IAzureServiceBusSender, AzureServiceBusSender>();
        return services;

    }
}
