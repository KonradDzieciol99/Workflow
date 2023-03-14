
using Mango.MessageBus;
using MediatR;
using MessageBus.Events;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
}).AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis"), options =>
{
    options.Configuration.ChannelPrefix = "SignalR";
});

var RedisOptions = new ConfigurationOptions()
{
    EndPoints = { { builder.Configuration.GetConnectionString("Redis") } },
    IncludeDetailInExceptions = true,
};

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(RedisOptions)
);///////////////////////////////na pewno singleton??????

builder.Services.AddAuthentication(opt =>
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
        ValidateAudience = false
    };
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

var CORSallowAny = "allowAny";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: CORSallowAny,
              policy =>
              {
                  policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
              });
});

//builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
    //opt.AddOpenBehavior(typeof(CollectUserEventsNotificationBehaviour<,>));
});
builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
        {
            {configuration.GetValue<string>("sendMessageToSignalRQueue"),typeof(SendMessageToSignalREvent)},
            {configuration.GetValue<string>("newOnlineUserWithFriendsQueue"),typeof(NewOnlineUserWithFriendsEvent)},
            {configuration.GetValue<string>("NewOnlineMessagesUserWithFriendsQueue"),typeof(NewOnlineMessagesUserWithFriendsEvent)},
            {configuration.GetValue<string>("NewOfflineUserWithFriendsQueue"),typeof(NewOfflineUserWithFriendsEvent)},
        //{configuration.GetValue<string>("FriendInvitationAcceptedQueue"),typeof(FriendInvitationAcceptedEvent)},
        //{configuration.GetValue<string>("InviteUserToFriendsQueue"),typeof(InviteUserToFriendsEvent)},
            {"notification-queue", typeof(NotificationEvent)}
        };
    opt.TopicNameAndEventTypePair = new Dictionary<string, Type>()
    {
        //{configuration.GetValue<string>("FriendInvitationAcceptedQueue"),typeof(FriendInvitationAcceptedEvent)},
        {configuration.GetValue<string>("FriendInvitationAcceptedTopic"),typeof(FriendInvitationAcceptedEvent)},
        //{configuration.GetValue<string>("NewOfflineUserWithFriendsQueue"),typeof(InviteUserToFriendsEvent)},
        {"invite-user-to-friends-topic",typeof(InviteUserToFriendsEvent)},
        //{configuration.GetValue<string>("NewOfflineUserTopic"),typeof(InviteUserToFriendsEvent)},
    };
    opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
    {
        {configuration.GetValue<string>("FriendInvitationAcceptedTopic"),"signalr"},
        {"invite-user-to-friends-topic","signalr"},
        //{configuration.GetValue<string>("FriendInvitationAcceptedQueue"),"signalr"},
        //{configuration.GetValue<string>("NewOfflineUserTopic"),"signalr"},
        //{configuration.GetValue<string>("NewOfflineUserWithFriendsQueue"),"signalr"},
        //{configuration.GetValue<string>("NewOfflineUserWithFriendsQueue"),"signalr"},
    };
});
builder.Services.AddAzureServiceBusSender(opt =>
{
    opt.ServiceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORSallowAny);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hub/Chat");

app.MapHub<PresenceHub>("/hub/Presence");

app.MapHub<MessagesHub>("/hub/Messages");

await app.RunAsync();
