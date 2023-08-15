using Chat.Infrastructure.DataAccess;
using MessageBus.Events;
using MessageBus;
using Microsoft.EntityFrameworkCore;
using Chat;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

await AddSubscriptions(app);

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

app.MapControllers();

app.Run();

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
