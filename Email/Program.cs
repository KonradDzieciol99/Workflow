using Email;
using Email.Extension;
using Email.MessageBus;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

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

builder.Services.AddSingleton<IEmailSender, EmailSender>(opt =>
{
    var verifyEmailUrl = builder.Configuration["VerifyEmailUrl"];
    var fluentEmailFactory = opt.GetRequiredService<IFluentEmailFactory>();
    return new EmailSender(fluentEmailFactory, verifyEmailUrl);
});

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.MapControllers();
//app.UseAuthorization();
app.UseAzureServiceBusConsumer();

app.Run();
