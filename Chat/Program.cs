using Chat;
using Chat.Application.IntegrationEvents;
using Chat.Infrastructure.DataAccess;
using Chat.Middleware;
using HealthChecks.UI.Client;
using Logging;
using MessageBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

builder.Host.UseSerilog(SeriLogger.Configure);

var app = builder.Build();

await AddSubscriptions(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}

app.UseCors("allowAny");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.Run();

async Task ApplyMigration() { 
    using var scope = app.Services.CreateScope();
    var initialiser = scope.ServiceProvider.GetRequiredService<SeedData>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
    return;
}

async Task AddSubscriptions(WebApplication app)
{
    var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

    var subscribeTasks = new List<Task>
    {
        eventBus.Subscribe<UserOnlineEvent>(),
        eventBus.Subscribe<UserOfflineEvent>(),
        eventBus.Subscribe<UserConnectedToChatEvent>(),
        eventBus.Subscribe<MarkChatMessageAsReadEvent>(),
    };

    await Task.WhenAll(subscribeTasks);
}


public partial class Program { }