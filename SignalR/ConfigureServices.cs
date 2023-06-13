using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using MessageBus.Extensions;

namespace SignalR;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddCors(opt =>
        {
            opt.AddPolicy(name: "allowAny",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                      });
        });

        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
            opt.SubscriptionName = "signalr";
        });

        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
        });

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });


        var redisConnString = configuration.GetConnectionString("Redis") ?? throw new ArgumentNullException("redisConnString");

        services.AddSignalR(o =>
        {
            o.EnableDetailedErrors = true;
        }).AddStackExchangeRedis(redisConnString, options =>
        {
            options.Configuration.ChannelPrefix = "SignalR";
        });

        var RedisOptions = new ConfigurationOptions()
        {
            EndPoints = { { redisConnString } },
            IncludeDetailInExceptions = true,
        };

        services.AddSingleton<IConnectionMultiplexer>(opt => ConnectionMultiplexer.Connect(RedisOptions));

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.RequireHttpsMetadata = false;
            opt.SaveToken = true;
            opt.Authority = "https://localhost:7122/";
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
            };
            opt.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();


        return services;
    }
}
