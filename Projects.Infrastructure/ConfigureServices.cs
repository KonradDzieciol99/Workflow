using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Projects.Application.Common.Interfaces;
using Projects.Infrastructure.DataAccess;
using Projects.Infrastructure.Repositories;
using Projects.Infrastructure.Services;

namespace Projects.Infrastructure;

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

        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));
        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));

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
        services.AddHealthChecks()
            .AddCheck("self",
            () => HealthCheckResult.Healthy(),
            tags: new string[] { "api" }
        )
        .AddAzureServiceBusTopic(
            configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
            configuration["AzureServiceBusSubscriberOptions:TopicName"],
            name: "projects-azure-service-bus-check",
            tags: new string[] { "azureServiceBus" }
        )
        .AddSqlServer(
            configuration["ConnectionStrings:DbContextConnString"],
            name: "projects-sql-db-check",
            tags: new string[] { "sql" })
        .AddIdentityServer(
            new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
            name: "tasks-identity-check",
            tags: new string[] { "identity" }
        );

        services.AddScoped<SeedData>();

        return services;
    }
}
