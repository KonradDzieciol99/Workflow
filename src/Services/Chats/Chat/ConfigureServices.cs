
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
            var internalIdentityUrl = configuration.GetValue<string>("urls:internal:identity") ?? throw new ArgumentNullException(nameof(configuration));
            var externalIdentityUrlhttp = configuration.GetValue<string>("urls:external:identity") ?? throw new ArgumentNullException(nameof(configuration));

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
                          policy.WithOrigins("https://localhost:4200",
                                             "http://localhost:4200",
                                             "http://localhost:1000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                      });
        });


        services.AddRabbitMQConsumer(configuration.GetSection("RabbitMQOptions"));
        services.AddRabbitMQSender(configuration.GetSection("RabbitMQOptions"));

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
        var test = configuration.GetValue("isTest", false);
        var test2 = configuration["isTest"];

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest",true))
            healthBuilder
                .AddCheck("self",
                    () => HealthCheckResult.Healthy(),
                    tags: new string[] { "api" }
                )
                .AddSqlServer(
                    configuration["ConnectionStrings:DbContextConnString"],
                    name: "chat-sql-db-check",
                    tags: new string[] { "sql" })
                .AddIdentityServer(
                    new Uri(configuration.GetValue<string>("urls:internal:identity")),
                    name: "chat-identity-check",
                    tags: new string[] { "identity" }
                );

        services.AddScoped<SeedData>();

        return services;
    }
}
