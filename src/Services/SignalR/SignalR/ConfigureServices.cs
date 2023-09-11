﻿using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;
using SignalR.Commons.Behaviours;
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
                          policy.WithOrigins("https://localhost:4200",
                                             "http://localhost:4200",
                                             "http://localhost:1000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                      });
        });

        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));

        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
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
             var internalIdentityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException("urls:internal:IdentityHttp");
             var externalIdentityUrlhttp = configuration.GetValue<string>("urls:external:IdentityHttp") ?? throw new ArgumentNullException("urls:external:IdentityHttp");
             var externalIdentityUrlhttps = configuration.GetValue<string>("urls:external:IdentityHttps") ?? throw new ArgumentNullException("urls:external:IdentityHttps");

             opt.RequireHttpsMetadata = false;
             opt.SaveToken = true;
             opt.Authority = internalIdentityUrl;
             opt.Audience = "signalR";

             opt.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 ValidateIssuerSigningKey = true,
                 ValidIssuers = new[] { externalIdentityUrlhttp, externalIdentityUrlhttps },
                 ClockSkew = TimeSpan.Zero

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "signalR");
            });
        });
        services.AddHealthChecks()
                .AddCheck("self",
                () => HealthCheckResult.Healthy(),
                tags: new string[] { "api" }
            )
            .AddAzureServiceBusTopic(
                configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
                configuration["AzureServiceBusSubscriberOptions:TopicName"],
                name: "signalr-azure-service-bus-check",
                tags: new string[] { "azureServiceBus" }
            )
            .AddRedis(
                configuration["ConnectionStrings:Redis"],
                name: "signalr-redis-check",
                tags: new string[] { "redis" })
            .AddIdentityServer(
                new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
                name: "signalr-identity-check",
                tags: new string[] { "identity" }
            );

        return services;
    }
}