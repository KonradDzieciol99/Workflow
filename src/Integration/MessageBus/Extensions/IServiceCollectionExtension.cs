using MessageBus.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageBus.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddAzureServiceBusConsumer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<AzureServiceBusConsumerOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IEventBusConsumer,AzureServiceBusConsumer>();
        services.AddHostedService(sp => (AzureServiceBusConsumer)sp.GetRequiredService<IEventBusConsumer>());
        return services;
    }
    public static IServiceCollection AddAzureServiceBusSender(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<AzureServiceBusSenderOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IEventBusSender, AzureServiceBusSender>();//scope ?
        return services;

    }
    public static IServiceCollection AddRabbitMQConsumer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<RabbitMQConsumerOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IEventBusConsumer,RabbitMQConsumer>();
        services.AddHostedService(sp => (RabbitMQConsumer)sp.GetRequiredService<IEventBusConsumer>());
        return services;
    }
    public static IServiceCollection AddRabbitMQSender(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<RabbitMQSenderOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddScoped<IEventBusSender, RabbitMQSender>();
        return services;

    }
}
