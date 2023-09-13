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
                connString = configuration.GetConnectionString("DbContextConnString") ?? throw new ArgumentNullException("DbContextConnString");
            else
                connString = configuration.GetConnectionString("NonDockerDbContextConnString") ?? throw new ArgumentNullException("NonDockerDbContextConnString");

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

        //services.AddLocalApiAuthentication();

        services.AddScoped<IEventSink, IdentityEvents>();
        services.AddScoped<SeedData>();

        services.AddRabbitMQSender(configuration.GetSection("RabbitMQConsumerOptions"));

        services.AddAuthentication()
        .AddOpenIdConnect("AzureOpenId", "Azure Active Directory OpenId", options =>
        {
            //options.Authority = "https://login.microsoftonline.com/" + configuration["AzureAd:TenantId"] + "/v2.0/";
            options.Authority = "https://login.microsoftonline.com/" + "5d3aafdf-d077-4419-bd3c-622d8000bc09" + "/v2.0/";
            options.ClientId = configuration["Authentication:Microsoft:ClientId"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.CallbackPath = "/signin-microsoft";
            //            options.UsePkce = _identityServerConfiguration.UsePkce;

            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

            //options.CallbackPath = "https://localhost:7122/signin-microsoft";
            //options.CallbackPath = configuration["AzureAd:CallbackPath"];
            options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
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
            options.ClientId = "944959518171-o303j7ij1434c3j1mrcl983qedr167b9.apps.googleusercontent.com";//todo
            options.ClientSecret = "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";//todo
            options.CallbackPath = "/signin-google";//todo
            options.GetClaimsFromUserInfoEndpoint = true;//We need this option
            options.RequireHttpsMetadata = false;
            options.SaveTokens = true;

            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("openid");
            options.ResponseType = OpenIdConnectResponseType.Code;
        })
        .AddJwtBearer(opt =>
        {
            var internalIdentityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException("urls:internal:IdentityHttp");
            var externalIdentityUrlhttp = configuration.GetValue<string>("urls:external:IdentityHttp") ?? throw new ArgumentNullException("urls:external:IdentityHttp");
            var externalIdentityUrlhttps = configuration.GetValue<string>("urls:external:IdentityHttps") ?? throw new ArgumentNullException("urls:external:IdentityHttps");

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
                ValidIssuers = new[] { externalIdentityUrlhttp, externalIdentityUrlhttps },
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IProfileService, ProfileService>();

        services.AddHealthChecks()
                    .AddCheck("self",
                    () => HealthCheckResult.Healthy(),
                    tags: new string[] { "api" }
                    )
                    .AddAzureServiceBusTopic(
                        configuration["AzureServiceBusSender:ServiceBusConnectionString"],
                        configuration["AzureServiceBusSender:TopicName"],
                        name: "projects-azure-service-bus-check",
                        tags: new string[] { "azureServiceBus" }
                    )
                    .AddSqlServer(
                        configuration["ConnectionStrings:DbContextConnString"],
                        name: "projects-sql-db-check",
                        tags: new string[] { "sql" })
                    .AddIdentityServer(
                        new Uri("https://accounts.google.com/"),
                        name: "identity-google-auth-check",
                        tags: new string[] { "identity" })
                    .AddIdentityServer(
                        new Uri("https://login.microsoftonline.com/common/v2.0/"),
                        name: "identity-microsoft-auth-check",
                        tags: new string[] { "identity" });


        return services;
    }
}
