
using Chat.Application.Common.Authorization.Handlers;
using Chat.Application.Common.Behaviours;
using Chat.Application.Common.Mappings;
using Chat.Domain.Services;
using Chat.Infrastructure.DataAccess;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using FluentValidation;
using MediatR;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace Chat;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IAuthorizationHandler, ShareFriendRequestRequirementHandler>();

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
            opt.Audience = "chat";

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
                policy.RequireClaim("scope", "chat");
            });
        });

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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

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


        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));
        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddTransient<IMessageService, MessageService>();

        services.AddHealthChecks()
            .AddCheck("self",
            () => HealthCheckResult.Healthy(),
            tags: new string[] { "api" }
        )
        .AddAzureServiceBusTopic(
            configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
            configuration["AzureServiceBusSubscriberOptions:TopicName"],
            name: "chat-azure-service-bus-check",
            tags: new string[] { "azureServiceBus" }
        )
        .AddSqlServer(
            configuration["ConnectionStrings:DbContextConnString"],
            name: "chat-sql-db-check",
            tags: new string[] { "sql" })
        .AddIdentityServer(
            new Uri(configuration.GetValue<string>("urls:internal:IdentityHttp")),
            name: "chat-identity-check",
            tags: new string[] { "identity" }
        );
        services.AddScoped<SeedData>();
        return services;
    }
}
