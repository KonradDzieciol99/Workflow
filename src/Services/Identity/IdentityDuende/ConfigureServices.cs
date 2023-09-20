using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityDuende.Configuration;
using IdentityDuende.Domain.DomainEvents;
using IdentityDuende.Domain.Entities;
using IdentityDuende.Infrastructure.DataAccess;
using IdentityDuende.Infrastructure.Repositories;
using IdentityDuende.Services;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityDuende;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddRazorPages()
            .AddViewOptions(options =>
            {
                options.HtmlHelperOptions.ClientValidationEnabled = true;
                options.HtmlHelperOptions.Html5DateRenderingMode = Html5DateRenderingMode.Rfc3339;

            });

        services.AddDbContext<ApplicationDbContext>(opt =>

        {
            string connString;
            var isDockerEnvironment = Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT");
            if (isDockerEnvironment is null || isDockerEnvironment == "true")
                connString = configuration.GetConnectionString("DbContextConnString") ?? throw new ArgumentNullException(nameof(configuration));
            else
                connString = configuration.GetConnectionString("NonDockerDbContextConnString") ?? throw new ArgumentNullException(nameof(configuration));

            opt.UseSqlServer(connString);
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;
            opt.User.RequireUniqueEmail = true;
            opt.SignIn.RequireConfirmedEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;

            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResource)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<ApplicationUser>();

        services.AddScoped<IEventSink, IdentityEvents>();
        services.AddScoped<SeedData>();

        services.AddRabbitMQSender(configuration.GetSection("RabbitMQConsumerOptions"));

        var authBuilder = services.AddAuthentication()
        .AddJwtBearer(opt =>
        {
            var internalIdentityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException(nameof(configuration));
            var externalIdentityUrlhttp = configuration.GetValue<string>("urls:external:IdentityHttp") ?? throw new ArgumentNullException(nameof(configuration));

            opt.RequireHttpsMetadata = false;
            opt.SaveToken = true;
            opt.Authority = internalIdentityUrl;
            opt.Audience = IdentityServerConstants.LocalApi.ScopeName;

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

        if (configuration.GetValue<bool>("externalAuth:enabled"))
        {
            authBuilder.AddOpenIdConnect("AzureOpenId", "Azure Active Directory OpenId", options =>
             {
                 options.Authority = "https://login.microsoftonline.com/" + configuration["externalAuth:Microsoft:Tenant"] + "/v2.0/";
                 options.ClientId = configuration["externalAuth:Microsoft:ClientId"];
                 options.ResponseType = OpenIdConnectResponseType.Code;
                 options.CallbackPath = "/signin-microsoft";
                 options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                 options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                 options.ClientSecret = configuration["externalAuth:Microsoft:ClientSecret"];
                 options.RequireHttpsMetadata = false;
                 options.SaveTokens = true;
                 options.GetClaimsFromUserInfoEndpoint = true;
                 options.Scope.Add("profile");
                 options.Scope.Add("email");
                 options.Scope.Add("openid");
                 options.Scope.Add("User.Read");
                 options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");

             }).AddOpenIdConnect("Google", "Sign-in with Google", options =>
             {
                 options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                 options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                 options.Authority = "https://accounts.google.com/";
                 options.ClientId = configuration["externalAuth:Microsoft:ClientId"];
                 options.ClientSecret = configuration["externalAuth:Microsoft:ClientSecret"]; ;
                 options.CallbackPath = "/signin-google";
                 options.GetClaimsFromUserInfoEndpoint = true;
                 options.RequireHttpsMetadata = false;
                 options.SaveTokens = true;
                 options.Scope.Add("profile");
                 options.Scope.Add("email");
                 options.Scope.Add("openid");
                 options.ResponseType = OpenIdConnectResponseType.Code;
             });
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IProfileService, ProfileService>();

        var healthChecksBuilder = services.AddHealthChecks()
                    .AddCheck("self",
                    () => HealthCheckResult.Healthy(),
                    tags: new string[] { "api" }
                    )
                    .AddSqlServer(
                        configuration["ConnectionStrings:DbContextConnString"],
                        name: "projects-sql-db-check",
                        tags: new string[] { "sql" });

        if (configuration.GetValue<bool>("externalAuth:enabled"))
        {
            healthChecksBuilder.AddIdentityServer(
                        new Uri("https://accounts.google.com/"),
                        name: "identity-google-auth-check",
                        tags: new string[] { "identity" })
                    .AddIdentityServer(
                        new Uri("https://login.microsoftonline.com/common/v2.0/"),
                        name: "identity-microsoft-auth-check",
                        tags: new string[] { "identity" });
        }

        return services;
    }
}
