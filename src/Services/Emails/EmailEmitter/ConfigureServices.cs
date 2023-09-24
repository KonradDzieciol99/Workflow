using EmailEmitter.Sender;
using MediatR;
using MessageBus.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EmailEmitter.Commons.Behaviours;

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
                        ?? throw new ArgumentNullException(nameof(configuration))
                )
                .AddRazorRenderer()
                .AddSendGridSender(
                    configuration["SendGrid:Key"]
                        ?? throw new ArgumentNullException(nameof(configuration))
                );

            services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQOptions"));
            services.AddRabbitMQSender(configuration.GetSection("RabbitMQOptions"));

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
                    new Uri(configuration.GetValue<string>("urls:internal:identity")),
                    name: "email-identity-check",
                    tags: new string[] { "identity" }
                );

            if (configuration.GetValue<bool>("SendGrid:enabled"))
                healthBuilder.AddSendGrid(
                    configuration["SendGrid:Key"],
                    name: "email-send-grid-check",
                    tags: new string[] { "sendGrid" }
                );
        }

        return services;
    }
}
