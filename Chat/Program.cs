using AutoMapper;
using Chat.Common.MapperProfiles;
using Chat.Persistence;
using Chat.Repositories;
using Mango.MessageBus;
using MediatR;
using MessageBus.Events;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContextConnString"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddMediatR(opt => 
{
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
    //opt.AddOpenBehavior(typeof(CollectUserEventsNotificationBehaviour<,>));
});
builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var myTuple = ("wartoœæ1", "wartoœæ2");
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
        {
            {configuration.GetValue<string>("markChatMessageAsReadQueue"),typeof(MarkChatMessageAsReadEvent)},
            {configuration.GetValue<string>("newOnlineUserQueue"),typeof(NewOnlineUserEvent)},
            //{configuration.GetValue<string>("FriendInvitationAcceptedQueue"),typeof(FriendInvitationAcceptedEvent)},
        };
    //opt.TopicNameWithSubscriptionNameAndEventTypePair = new Dictionary<Tuple<string, string>, Type>()
    //{
    //    {Tuple.Create(configuration.GetValue<string>("AzureBusTopic"),configuration.GetValue<string>("AzureBusSubscription")),typeof(NewUserRegisterCreateUser)},
    //    {Tuple.Create(configuration.GetValue<string>("newOfflineUserTopic"),configuration.GetValue<string>("newOfflineUserTopicChatSub")),typeof(NewOfflineUserEvent)},
    //};
    opt.TopicNameAndEventTypePair = new Dictionary<string, Type>()
    {
        //{configuration.GetValue<string>("AzureBusTopic"),typeof(NewUserRegisterCreateUser)},
        {configuration.GetValue<string>("newOfflineUserTopic"),typeof(NewOfflineUserEvent)},
    };
    opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
    {
        //{configuration.GetValue<string>("AzureBusTopic"),configuration.GetValue<string>("AzureBusSubscription")},
        {configuration.GetValue<string>("newOfflineUserTopic"),configuration.GetValue<string>("newOfflineUserTopicChatSub")},
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
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            if (initialiser.Database.IsSqlServer())
                await initialiser.Database.MigrateAsync();
        }
        catch (Exception)
        {
            //_logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}

app.UseHttpsRedirection();

app.UseCors(CORSallowAny);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
