using Duende.IdentityServer.Services;
using Duende.IdentityServer;
using IdentityDuende.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityDuende.Events;
using IdentityDuende.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using MessageBus.Extensions;
using System.Security.Claims;
using IdentityDuende.Infrastructure.Repositories;
using IdentityDuende.Services;
using IdentityDuende.Configuration;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityDuende;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

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

        services.AddLocalApiAuthentication();

        services.AddScoped<IEventSink, IdentityEvents>();
        services.AddScoped<SeedData>();

        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException("ServiceBusConnectionString");
        });

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
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IProfileService, ProfileService>();

        return services;
    }
}
