using EmailEmitter.Sender;
using MessageBus.Extensions;
namespace EmailEmitter;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddFluentEmail(configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException("EmailConfiguration:From"))
                .AddRazorRenderer()
                .AddSendGridSender(configuration["SendGrid:Key"] ?? throw new ArgumentNullException("SendGrid:Key"));
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        services.AddScoped<ISenderSource, SenderSource>();
        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));
        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));
        return services;
    }
}
