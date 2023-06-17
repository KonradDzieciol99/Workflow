using EmailSender;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MessageBus.Extensions;

namespace IdentityDuende;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddFluentEmail(configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException("EmailConfiguration:From"))
                .AddRazorRenderer()
                .AddSendGridSender(configuration["SendGrid_Key"] ?? throw new ArgumentNullException("SendGrid_Key"));
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
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            opt.SubscriptionName = "email-sender";
        });
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
        });
        return services;
    }
}
