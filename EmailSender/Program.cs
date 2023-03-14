using EmailSender;
using EmailSender.Extension;
using EmailSender.MessageBus;
using FluentEmail.Core;
using MessageBus.Events;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.Authority = "https://localhost:7122/";
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});
var from = builder.Configuration["EmailConfiguration:From"];
var key = builder.Configuration["SendGrid_Key"];

builder.Services.AddFluentEmail(from)
        .AddRazorRenderer()
        .AddSendGridSender(key);
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

//builder.Services.AddSingleton<IEmailSender, EmailSenderS>(opt =>
//{
//    var verifyEmailUrl = builder.Configuration["VerifyEmailUrl"];
//    var from = builder.Configuration["EmailConfiguration:From"];
//    var fluentEmailFactory = opt.GetRequiredService<IFluentEmailFactory>();
//    return new EmailSenderS(fluentEmailFactory, verifyEmailUrl, from);
//});

//builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddScoped<ISender, Sender>();
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
    };
    opt.TopicNameWithSubscriptionName = new Dictionary<string, string>()
    {
        {configuration.GetValue<string>("NewUserRegistrationEvent"),configuration.GetValue<string>("NewUserRegistrationEventSubscription")},
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
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
//app.UseAzureServiceBusConsumer();
app.Run();
