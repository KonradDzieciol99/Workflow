using MediatR;
using MessageBus.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EmailEmitter.Commons.Behaviours;
using EmailEmitter.Services;

namespace EmailEmitter;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (configuration.GetValue<bool>("SendGrid:enabled"))
        {
            services
                .AddFluentEmail(
                    configuration["EmailConfiguration:From"]
                        ?? throw new InvalidOperationException("The expected configuration value 'EmailConfiguration:From' is missing.")
                )
                .AddRazorRenderer()
                .AddSendGridSender(
                    configuration["SendGrid:Key"]
                        ?? throw new InvalidOperationException("The expected configuration value 'SendGrid:Key' is missing.")
                );

            services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQOptions") ?? throw new InvalidOperationException("The expected configuration value 'RabbitMQOptions' is missing."));
            services.AddRabbitMQSender(configuration.GetSection("RabbitMQOptions") ?? throw new InvalidOperationException("The expected configuration value 'RabbitMQOptions' is missing."));

            services.AddScoped<ISenderSource, SenderSource>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(UnhandledExceptionBehaviour<,>)
                );
            });
        }

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
        {
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddIdentityServer(
                    new Uri(configuration.GetValue<string>("urls:internal:identity") ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:identity' is missing.")),
                    name: "email-identity-check",
                    tags: new string[] { "identity" }
                );

            if (configuration.GetValue<bool>("SendGrid:enabled"))
                healthBuilder.AddSendGrid(
                    configuration["SendGrid:Key"] ?? throw new InvalidOperationException("The expected configuration value 'SendGrid:Key' is missing."),
                    name: "email-send-grid-check",
                    tags: new string[] { "sendGrid" }
                );
        }

        return services;
    }
}
