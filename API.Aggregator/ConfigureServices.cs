using API.Aggregator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

namespace API.Aggregator;

public static class ConfigureServices
{
    public static IServiceCollection AddAggregatorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpClient<IIdentityServerService, IdentityServerService>();
        services.AddHttpClient<ITaskService, TaskService>();
        services.AddHttpClient<IProjectsService, ProjectsService>();
        services.AddHttpClient<IChatService, ChatService>();

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
            opt.Audience = "aggregator";

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
                policy.RequireClaim("scope", "aggregator");
            });
        });

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

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new string[] { "api" })
            .AddUrlGroup(new Uri($"{configuration["urls:internal:chat"]}/hc"), name: "chat-check", tags: new string[] { "chat" })
            .AddUrlGroup(new Uri($"{configuration["urls:internal:IdentityHttp"]}/hc"), name: "identity-check", tags: new string[] { "identity" })
            .AddUrlGroup(new Uri($"{configuration["urls:internal:notificationHttp"]}/hc"), name: "notification-check", tags: new string[] { "notification" })
            .AddUrlGroup(new Uri($"{configuration["urls:internal:projectsHttp"]}/hc"), name: "projects-check", tags: new string[] { "projects" })
            .AddUrlGroup(new Uri($"{configuration["urls:internal:tasksHttp"]}/hc"), name: "task-check", tags: new string[] { "task" });

        return services;
    }
}
