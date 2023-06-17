using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Projects.Application.Common.Interfaces;
using Projects.Infrastructure.Services;
using Projects.Infrastructure.Repositories;
using Projects.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using MessageBus.Extensions;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString") ?? throw new ArgumentNullException("DbContextConnString")) ;
        });
        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
        });

        services.AddAzureServiceBusSubscriber(opt =>
        {
            //var configuration = builder.Configuration;
            opt.ServiceBusConnectionString = configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
            opt.SubscriptionName = "projects";
        });

        services.AddCors(opt =>
        {
            opt.AddPolicy(name: "allowAny", policy =>
            {
                policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
            });
        });


        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt => {
            opt.RequireHttpsMetadata = false;
            opt.SaveToken = true;
            opt.Authority = "https://localhost:7122/";
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "project");
            });
        });


        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("ProjectMembershipPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectMembershipRequirement()
        //    ));
        //    options.AddPolicy("ProjectManagementPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectManagementRequirement()
        //        ));
        //    options.AddPolicy("ProjectAuthorPolicy", policy =>
        //    policy.AddRequirements(
        //        new ProjectAuthorRequirement()
        //        ));
        //});

        return services;
    }
}
