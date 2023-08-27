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
        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetConnectionString("ServiceBus") ?? throw new ArgumentNullException("ServiceBus");
            opt.SubscriptionName = "email-sender";
        });
        services.AddAzureServiceBusSender(opt => configuration.GetConnectionString("ServiceBus"));
        return services;
    }
}
