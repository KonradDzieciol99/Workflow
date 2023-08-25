﻿using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Projects.Application.Common.Interfaces;
using Projects.Infrastructure.DataAccess;
using Projects.Infrastructure.Repositories;
using Projects.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            string connString;
            var isDockerEnvironment = Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT");
            if (isDockerEnvironment is null || isDockerEnvironment == "true")
                connString = configuration.GetConnectionString("DbContextConnString") ?? throw new ArgumentNullException("DbContextConnString");
            else
                connString = configuration.GetConnectionString("NonDockerDbContextConnString") ?? throw new ArgumentNullException("NonDockerDbContextConnString");

            opt.UseSqlServer(connString);
        });

        //services.AddDbContext<ApplicationDbContext>(opt =>
        //{
        //    opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString") ?? throw new ArgumentNullException("DbContextConnString")) ;
        //});


        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
        });

        services.AddAzureServiceBusSubscriber(opt =>
        {
            //var configuration = builder.Configuration;
            opt.ServiceBusConnectionString = configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
            opt.SubscriptionName = "projects";
        });

        services.AddCors(opt =>
        {
            opt.AddPolicy(name: "allowAny", policy =>
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
            opt.Audience = "project";

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "project");
            });
        });


        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("ProjectMembershipPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectMembershipRequirement()
        //    ));
        //    options.AddPolicy("ProjectManagementPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectManagementRequirement()
        //        ));
        //    options.AddPolicy("ProjectAuthorPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectAuthorRequirement()
        //        ));
        //});

        return services;
    }
}
