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
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            var connString =
                configuration.GetConnectionString("DbContextConnString")
                ?? throw new ArgumentNullException(nameof(configuration));
            opt.UseSqlServer(connString);
        });

        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQOptions"));
        services.AddRabbitMQSender(configuration.GetSection("RabbitMQOptions"));

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
                    ?? throw new ArgumentNullException(nameof(configuration));
                var externalIdentityUrlhttp =
                    configuration.GetValue<string>("urls:external:identity")
                    ?? throw new ArgumentNullException(nameof(configuration));

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
                    policy.RequireClaim("scope", "project");
                }
            );
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
        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddSqlServer(
                    configuration["ConnectionStrings:DbContextConnString"],
                    name: "projects-sql-db-check",
                    tags: new string[] { "sql" }
                )
                .AddIdentityServer(
                    new Uri(configuration.GetValue<string>("urls:internal:identity")),
                    name: "tasks-identity-check",
                    tags: new string[] { "identity" }
                );

        services.AddScoped<SeedData>();

        return services;
    }
}
