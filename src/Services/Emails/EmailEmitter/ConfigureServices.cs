using EmailEmitter.Sender;
using MediatR;
using MessageBus.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EmailEmitter.Commons.Behaviours;

namespace EmailEmitter;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddFluentEmail(configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException("EmailConfiguration:From"))
                .AddRazorRenderer()
                .AddSendGridSender(configuration["SendGrid:Key"] ?? throw new ArgumentNullException("SendGrid:Key"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

        });

        services.AddScoped<ISenderSource, SenderSource>();

        services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQConsumerOptions"));
        services.AddRabbitMQSender(configuration.GetSection("RabbitMQConsumerOptions"));

        services.AddHealthChecks()
            .AddCheck("self",
                () => HealthCheckResult.Healthy(),
                tags: new string[] { "api" }
            )
            .AddAzureServiceBusTopic(
                configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
                configuration["AzureServiceBusSubscriberOptions:TopicName"],
                name: "email-azure-service-bus-check",
                tags: new string[] { "azureServiceBus" }
            )
            .AddSendGrid(
                configuration["SendGrid:Key"],
                name: "email-send-grid-check",
                tags: new string[] { "sendGrid" })
            .AddIdentityServer(
                new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
                name: "email-identity-check",
                tags: new string[] { "identity" }
            ); 

        return services;
    }
}
