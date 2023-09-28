using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Photos.Infrastructure.DataAccess;
using HttpMessage.Behaviours;
using MediatR.Pipeline;

namespace Photos;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

        services.AddSingleton(opt =>
        {
            return new BlobServiceClient(
                configuration.GetConnectionString("AzureStorage")
                    ?? throw new InvalidOperationException(
                        "The expected configuration value 'ConnectionStrings:AzureStorage' is missing."
                    )
            );
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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
                opt.Audience = "photos";

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

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "ApiScope",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "photos");
                }
            );
        });

        var healthBuilder = services.AddHealthChecks();

        if (!configuration.GetValue("isTest", true))
            healthBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
                //.AddAzureBlobStorage(configuration.GetConnectionString("AzureStorage"),
                //    name: "photos-azure-blob-storage-check",
                //    tags: new string[] { "azureServiceBus" })
                .AddIdentityServer(
                    new Uri(
                        configuration.GetValue<string>("urls:internal:identity")
                            ?? throw new InvalidOperationException(
                                "The expected configuration value 'urls:internal:identity' is missing."
                            )
                    ),
                    name: "photos-identity-check",
                    tags: new string[] { "identity" }
                );

        services.AddScoped<SeedData>();

        return services;
    }
}
