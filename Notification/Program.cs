using Mango.MessageBus;
using MessageBus.Events;
using MessageBus.Extensions;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Amazon.Runtime.Internal.Transform;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var connectionString = builder.Configuration.GetConnectionString("MongoDB");
//builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
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
        ValidateAudience = false,
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

builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    var client = new MongoClient(connectionString);
    var databaseName = builder.Configuration.GetValue<string>("MongoDb:DatabaseName");
    return client.GetDatabase(databaseName);
});

builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
        {
        };
    opt.TopicNameAndEventTypePair = new Dictionary<string, Type>()
    {
        {configuration.GetValue<string>("NewUserRegistrationEvent"),typeof(NewUserRegistrationEvent)},
        {"invite-user-to-friends-topic",typeof(InviteUserToFriendsEvent)},
        {"friend-invitation-accepted-topic",typeof(FriendInvitationAcceptedEvent)},
    };
    opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
    {
        {configuration.GetValue<string>("NewUserRegistrationEvent"),configuration.GetValue<string>("NewUserRegistrationEventSubscription")},
        {"invite-user-to-friends-topic","notification"},
        {"friend-invitation-accepted-topic","notification"},
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

await app.RunAsync();



//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
