using MessageBus;
using MessageBus.Events;
using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.DataAccess;
using Notification.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

var subscribeTasks = new List<Task>
{
    eventBus.Subscribe<RegistrationEvent>(),
    eventBus.Subscribe<FriendRequestAddedEvent>(),
    eventBus.Subscribe<FriendRequestAcceptedEvent>(),
    eventBus.Subscribe<FriendRequestRemovedEvent>(),
    eventBus.Subscribe<UserOnlineEvent>(),
    eventBus.Subscribe<ProjectMemberAddedEvent>(),
    eventBus.Subscribe<ProjectMemberAcceptInvitationEvent>(),
    eventBus.Subscribe<ProjectMemberDeclineInvitationEvent>(),
};

await Task.WhenAll(subscribeTasks);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}

//app.UseHttpsRedirection();
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
