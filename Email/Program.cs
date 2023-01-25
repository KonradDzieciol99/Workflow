using Email;
using Email.Extension;
using Email.MessageBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
//app.UseAuthorization();
app.UseAzureServiceBusConsumer();

app.Run();
