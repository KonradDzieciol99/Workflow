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
        if (configuration.GetValue<bool>("SendGrid:enabled"))
        {
            services.AddFluentEmail(configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException(nameof(configuration)))
                .AddRazorRenderer()
                .AddSendGridSender(configuration["SendGrid:Key"] ?? throw new ArgumentNullException(nameof(configuration)));

            services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQConsumerOptions"));
            services.AddRabbitMQSender(configuration.GetSection("RabbitMQConsumerOptions"));

            services.AddScoped<ISenderSource, SenderSource>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            });
        }

        var healthChecksBuilder =services.AddHealthChecks()
            .AddCheck("self",
                () => HealthCheckResult.Healthy(),
                tags: new string[] { "api" }
            )
            .AddIdentityServer(
                    new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
                    name: "email-identity-check",
                    tags: new string[] { "identity" }
             );

        if (configuration.GetValue<bool>("SendGrid:enabled"))
        {
            healthChecksBuilder
                .AddSendGrid(
                    configuration["SendGrid:Key"],
                    name: "email-send-grid-check",
                    tags: new string[] { "sendGrid" });
        }

        return services;
    }
}
