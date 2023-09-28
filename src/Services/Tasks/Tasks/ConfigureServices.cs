using FluentValidation;
using MediatR;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Tasks.Application.Common.Authorization.Handlers;
using Tasks.Common;
using Tasks.Domain.Services;
using Tasks.Infrastructure.DataAccess;
using Tasks.Infrastructure.Repositories;
using HttpMessage.Behaviours;
using HttpMessage.Services;
using MediatR.Pipeline;

namespace Tasks;

public static class ConfigureServices
{
    public static IServiceCollection AddTasksServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IAuthorizationHandler, ProjectMembershipRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectManagmentOrTaskAuthorRequirementHandler>();

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
                opt.Audience = "tasks";

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
                    policy.RequireClaim("scope", "tasks");
                }
            );
        });

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(
                configuration.GetConnectionString("DbContextConnString")
                    ?? throw new InvalidOperationException(
                        "The expected configuration value 'ConnectionStrings:DbContextConnString' is missing."
                    )
            );
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

        services.AddRabbitMQConsumer(
            configuration.GetSection("RabbitMQOptions")
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'RabbitMQOptions' is missing."
                )
        );
        services.AddRabbitMQSender(
            configuration.GetSection("RabbitMQOptions")
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'RabbitMQOptions' is missing."
                )
        );

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
        });

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddTransient<IAppTaskService, AppTaskService>();

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                .AddSqlServer(
                    configuration["ConnectionStrings:DbContextConnString"]
                        ?? throw new InvalidOperationException(
                            "The expected configuration value 'ConnectionStrings:DbContextConnString' is missing."
                        ),
                    name: "tasks-sql-db-check",
                    tags: new string[] { "sql" }
                )
                .AddIdentityServer(
                    new Uri(
                        configuration.GetValue<string>("urls:internal:identity")
                            ?? throw new InvalidOperationException(
                                "The expected configuration value 'urls:internal:identity' is missing."
                            )
                    ),
                    name: "tasks-identity-check",
                    tags: new string[] { "identity" }
                );

        services.AddScoped<SeedData>();

        return services;
    }
}

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
    }
}
