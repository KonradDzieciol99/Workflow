using EmailSender;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MessageBus.Extensions;

namespace IdentityDuende;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x => {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.Authority = "https://localhost:7122/";
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
        var from = configuration["EmailConfiguration:From"];
        var key = configuration["SendGrid_Key"];

        services.AddFluentEmail(from)
                .AddRazorRenderer()
                .AddSendGridSender(key);
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

        //builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
        services.AddScoped<ISender, Sender>();
        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            opt.SubscriptionName = "email-sender";
            //opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
            //{
            //};
            //opt.TopicNameAndEventTypePair = new Dictionary<string, Type>()
            //{
            //    {configuration.GetValue<string>("NewUserRegistrationEvent"),typeof(NewUserRegistrationEvent)},
            //};
            //opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
            //{
            //    {configuration.GetValue<string>("NewUserRegistrationEvent"),configuration.GetValue<string>("NewUserRegistrationEventSubscription")},
            //};
        });
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
        });
        return services;
    }
}
