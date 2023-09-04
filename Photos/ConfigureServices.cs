﻿using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Photos.Application.Common.Behaviours;

namespace Photos;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

        services.AddScoped(opt =>
        {
            return new BlobServiceClient(configuration.GetConnectionString("AzureStorage") ?? throw new ArgumentNullException("AzureStorage"));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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
            opt.Audience = "photos";

            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuers = new[] { externalIdentityUrlhttp, externalIdentityUrlhttps },
                ClockSkew = TimeSpan.Zero

            };
        });

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "photos");
            });
        });

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(),
            tags: new string[] { "api" }
            )
            .AddAzureBlobStorage(configuration.GetConnectionString("AzureStorage"),
                name: "photos-azure-blob-storage-check",
                tags: new string[] { "azureServiceBus" })
            .AddIdentityServer(
                new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
                name: "photos-identity-check",
                tags: new string[] { "identity" }
            );


        return services;
    }
}
