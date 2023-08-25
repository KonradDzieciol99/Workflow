using EmailSender.Sender;
using MessageBus.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

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

        //builder.Services.AddSingleton<IEmailSender, EmailSenderS>(opt =>
        //{
        //    var verifyEmailUrl = builder.Configuration["VerifyEmailUrl"];
        //    var from = builder.Configuration["EmailConfiguration:From"];
        //    var fluentEmailFactory = opt.GetRequiredService<IFluentEmailFactory>();
        //    return new EmailSenderS(fluentEmailFactory, verifyEmailUrl, from);
        //});

        services.AddScoped<ISender, Sender>();
        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetConnectionString("ServiceBus") ?? throw new ArgumentNullException("ServiceBus");
            opt.SubscriptionName = "email-sender";
        });
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetConnectionString("ServiceBus") ?? throw new ArgumentNullException("ServiceBus");
        });
        return services;
    }
}
