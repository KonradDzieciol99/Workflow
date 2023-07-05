
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Infrastructure.DataAccess;
using Chat.Infrastructure.Repositories;
using Chat.Application.Common.Mappings;
using Chat.Application.Common.Behaviours;
using Chat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Chat.Application.Common.Authorization.Handlers;
using Chat.Domain.Services;
using Microsoft.Extensions.Options;

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
            var internalIdentityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException("urls:internal:IdentityHttp");
            var externalIdentityUrlhttp = configuration.GetValue<string>("urls:external:IdentityHttp") ?? throw new ArgumentNullException("urls:external:IdentityHttp");
            var externalIdentityUrlhttps = configuration.GetValue<string>("urls:external:IdentityHttps") ?? throw new ArgumentNullException("urls:external:IdentityHttps");

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
                ValidIssuers = new[] { externalIdentityUrlhttp, externalIdentityUrlhttps },
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

        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
            opt.SubscriptionName = "chat";
        });

        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
        });

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


        return services;
    }
}
