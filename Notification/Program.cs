using MessageBus.Events;
using MessageBus;
using Notification;
using Notification.Middleware;
using Notification.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();// nie potrzeba tworzyæ scope bo to singletone

var subscribeTasks = new List<Task>
{
    eventBus.Subscribe<NewUserRegistrationEvent>(),
    eventBus.Subscribe<FriendRequestAddedEvent>(),
    eventBus.Subscribe<FriendRequestAcceptedEvent>(),
    eventBus.Subscribe<FriendRequestRemovedEvent>(),
};

await Task.WhenAll(subscribeTasks);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();

}

app.UseHttpsRedirection();
app.UseCors("allowAny");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

await app.RunAsync();

async Task ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            if (initialiser.Database.IsSqlServer())
                await initialiser.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    await Task.CompletedTask;
    return;
}
