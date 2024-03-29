﻿using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using MediatR;
using API.Aggregator.Infrastructure.Services;
using API.Aggregator.Infrastructure;
using Polly;
using HttpMessage.Behaviours;
using HttpMessage.Services;
using MediatR.Pipeline;

namespace API.Aggregator;

public static class ConfigureServices
{
    public static IServiceCollection AddAggregatorServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services
            .AddHttpClient("InternalHttpClient")
            .AddHttpMessageHandler<HttpClientTokenForwarderDelegatingHandler>()
            //.AddHttpMessageHandler<HttpClientErrorHandlingDelegatingHandler>()
            .AddTransientHttpErrorPolicy(builder =>
            {
                return builder.WaitAndRetryAsync(
                    retryCount: 4,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt)
                );
            });

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IIdentityServerService, IdentityServerService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IPhotosService, PhotosService >();

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
                opt.Audience = "aggregator";

                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = new[] { externalIdentityUrlhttp },
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "ApiScope",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "aggregator");
                }
            );
        });

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

        var healthBuilder = services.AddHealthChecks();
        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddUrlGroup(
                    new Uri($"{configuration["urls:internal:chat"]}/hc"),
                    name: "chat-check",
                    tags: new string[] { "chat" }
                )
                .AddUrlGroup(
                    new Uri($"{configuration["urls:internal:identity"]}/hc"),
                    name: "identity-check",
                    tags: new string[] { "identity" }
                )
                .AddUrlGroup(
                    new Uri($"{configuration["urls:internal:notification"]}/hc"),
                    name: "notification-check",
                    tags: new string[] { "notification" }
                )
                .AddUrlGroup(
                    new Uri($"{configuration["urls:internal:projects"]}/hc"),
                    name: "projects-check",
                    tags: new string[] { "projects" }
                )
                .AddUrlGroup(
                    new Uri($"{configuration["urls:internal:tasks"]}/hc"),
                    name: "task-check",
                    tags: new string[] { "task" }
                );

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
        });

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();
        services.AddTransient<HttpClientTokenForwarderDelegatingHandler>();
        //services.AddTransient<HttpClientErrorHandlingDelegatingHandler>();

        return services;
    }
}
