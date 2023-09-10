
using FluentValidation;
using MediatR;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Tasks.Application.Common.Authorization.Handlers;
using Tasks.Application.Common.Behaviours;
using Tasks.Common;
using Tasks.Domain.Services;
using Tasks.Infrastructure.DataAccess;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;

namespace Tasks;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IAuthorizationHandler, ProjectMembershipRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectManagmentOrTaskAuthorRequirementHandler>();

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
            opt.Audience = "tasks";

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
                policy.RequireClaim("scope", "tasks");
            });
        });

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
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

        services.AddAzureServiceBusSubscriber(configuration.GetSection("AzureServiceBusSubscriberOptions"));

        services.AddAzureServiceBusSender(configuration.GetSection("AzureServiceBusSender"));

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

        services.AddTransient<IAppTaskService, AppTaskService>();

        services.AddHealthChecks()
                    .AddCheck("self",() => HealthCheckResult.Healthy(),
                        tags: new string[] { "api" }
                    )
                    .AddAzureServiceBusTopic(
                        configuration["AzureServiceBusSubscriberOptions:ServiceBusConnectionString"],
                        configuration["AzureServiceBusSubscriberOptions:TopicName"],
                        name: "tasks-azure-service-bus-check",
                        tags: new string[] { "azureServiceBus" }
                    )
                    .AddSqlServer(
                        configuration["ConnectionStrings:DbContextConnString"],
                        name: "tasks-sql-db-check",
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
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
    }
}