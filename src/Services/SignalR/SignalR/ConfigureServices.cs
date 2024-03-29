﻿using MediatR.Pipeline;
using MediatR;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;
using HttpMessage.Behaviours;

namespace SignalR;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(
                name: "allowAny",
                policy =>
                {
                    policy
                        .WithOrigins(
                            "https://localhost:4200",
                            "http://localhost:4200",
                            "http://localhost:1000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });
        var rabbitMQOptionsSection =
            configuration.GetSection("RabbitMQOptions")
            ?? throw new InvalidOperationException(
                "The expected configuration value 'RabbitMQOptions' is missing."
            );

        services.AddRabbitMQConsumer(rabbitMQOptionsSection);
        services.AddRabbitMQSender(rabbitMQOptionsSection);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
        });

        var redisConnString =
            configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException(
                "The expected configuration value 'ConnectionStrings:Redis' is missing."
            );

        services
            .AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            })
            .AddStackExchangeRedis(
                redisConnString,
                options =>
                {
                    options.Configuration.ChannelPrefix = "SignalR";
                }
            );

        var RedisOptions = new ConfigurationOptions()
        {
            EndPoints = { { redisConnString } },
            IncludeDetailInExceptions = true,
        };

        services.AddSingleton<IConnectionMultiplexer>(
            opt => ConnectionMultiplexer.Connect(RedisOptions)
        );

        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                var internalIdentityUrl =
                    configuration.GetValue<string>("urls:internal:identity")
                    ?? throw new InvalidOperationException(
                        "The expected configuration value 'urls:internal:identity' is missing."
                    );
                var externalIdentityUrlhttp =
                    configuration.GetValue<string>("urls:external:identity")
                    ?? throw new InvalidOperationException(
                        "The expected configuration value 'urls:external:identity' is missing."
                    );

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
                    ValidIssuers = new[] { externalIdentityUrlhttp },
                    ClockSkew = TimeSpan.Zero
                };
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "ApiScope",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "signalR");
                }
            );
        });

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddRedis(
                    configuration["ConnectionStrings:Redis"]
                        ?? throw new InvalidOperationException(
                            "The expected configuration value 'ConnectionStrings:Redis' is missing."
                        ),
                    name: "signalr-redis-check",
                    tags: new string[] { "redis" }
                )
                .AddIdentityServer(
                    new Uri(
                        configuration.GetValue<string>("urls:internal:identity")
                            ?? throw new InvalidOperationException(
                                "The expected configuration value 'urls:internal:identity' is missing."
                            )
                    ),
                    name: "signalr-identity-check",
                    tags: new string[] { "identity" }
                );

        return services;
    }
}
