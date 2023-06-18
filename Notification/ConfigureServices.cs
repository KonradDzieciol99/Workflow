using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MessageBus.Extensions;
using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.DataAccess;
using MediatR;
using Notification.Application.Common.Behaviours;
using Notification.Infrastructure.Repositories;
using Notification.Services;

namespace Notification;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
            opt.Audience = "notification";

            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuers = new[] { externalIdentityUrlhttp, externalIdentityUrlhttps },
            };
        });

        services.AddCors(opt =>
        {
            opt.AddPolicy("allowAny", policy =>
                      {
                          policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                      });
        });
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        services.AddAzureServiceBusSubscriber(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException("ServiceBusConnectionString"); ;
            opt.SubscriptionName = "notification";
            //opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
            //    {
            //    };
            //opt.TopicNameAndEventTypePair = new Dictionary<string, Type>()
            //{
            //    {configuration.GetValue<string>("NewUserRegistrationEvent"),typeof(NewUserRegistrationEvent)},
            //    {"invite-user-to-friends-topic",typeof(FriendInvitationAddedEvent)},
            //    {"friend-invitation-accepted-topic",typeof(FriendInvitationAcceptedEvent)},
            //};
            //opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
            //{
            //    {configuration.GetValue<string>("NewUserRegistrationEvent"),configuration.GetValue<string>("NewUserRegistrationEventSubscription")},
            //    {"invite-user-to-friends-topic","notification"}, 
            //    {"friend-invitation-accepted-topic","notification"},
            //};
        });
        services.AddAzureServiceBusSender(opt =>
        {
            opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString")  ?? throw new ArgumentNullException("ServiceBusConnectionString");
        });
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "notification");
            });
        });

        return services;
    }

}
