using IdentityDuende;
using MessageBus;
using MessageBus.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

await eventBus.Subscribe<NewUserRegistrationEvent>();

if (app.Environment.IsDevelopment()){}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
