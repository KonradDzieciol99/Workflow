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
using static System.Net.Mime.MediaTypeNames;

namespace Chat;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IAuthorizationHandler, ShareFriendRequestRequirementHandler>();

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
                        ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:identity' is missing.");
                var externalIdentityUrlhttp =
                    configuration.GetValue<string>("urls:external:identity")
                        ?? throw new InvalidOperationException("The expected configuration value 'urls:external:identity' is missing.");

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
                    policy.RequireClaim("scope", "chat");
                }
            );
        });
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString") ?? throw new InvalidOperationException("The expected configuration value 'ConnectionStrings:DbContextConnString' is missing."));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

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

        services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQOptions") ?? throw new InvalidOperationException("The expected configuration value 'RabbitMQOptions' is missing."));
        services.AddRabbitMQSender(configuration.GetSection("RabbitMQOptions") ?? throw new InvalidOperationException("The expected configuration value 'RabbitMQOptions' is missing."));

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

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddSqlServer(
                    configuration["ConnectionStrings:DbContextConnString"] ?? throw new InvalidOperationException("The expected configuration value 'ConnectionStrings:DbContextConnString' is missing."),
                    name: "chat-sql-db-check",
                    tags: new string[] { "sql" }
                )
                .AddIdentityServer(
                    new Uri(configuration.GetValue<string>("urls:internal:identity") ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:identity' is missing.")),
                    name: "chat-identity-check",
                    tags: new string[] { "identity" }
                );

        services.AddScoped<SeedData>();

        return services;
    }
}
