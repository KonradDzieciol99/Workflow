using MediatR;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Notification.Application.Common.Authorization.Handlers;
using Notification.Application.Common.Behaviours;
using Notification.Infrastructure.DataAccess;
using Notification.Infrastructure.Repositories;
using Notification.Services;

namespace Notification;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
            opt.Audience = "notification";

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

        services.AddCors(opt =>
        {
            opt.AddPolicy("allowAny", policy =>
                      {
                          policy.WithOrigins("https://localhost:4200",
                                             "http://localhost:4200",
                                             "http://localhost:1000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                      });
        });
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));
        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "notification");
            });
        });

        services.AddScoped<IAuthorizationHandler, ProjectMembershipRequirementHandler>();

        services.AddHealthChecks()
                    .AddCheck("self",() => HealthCheckResult.Healthy(),
                        tags: new string[] { "api" }
                    )
                    .AddAzureServiceBusTopic(
                        configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
                        configuration["AzureServiceBusSubscriberOptions:TopicName"],
                        name: "notification-azure-service-bus-check",
                        tags: new string[] { "azureServiceBus" }
                    )
                    .AddSqlServer(
                        configuration["ConnectionStrings:DbContextConnString"],
                        name: "notification-sql-db-check",
                        tags: new string[] { "sql" })
                    .AddIdentityServer(
                        new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
                        name: "notification-identity-check",
                        tags: new string[] { "identity" }
                    );

        return services;
    }

}
