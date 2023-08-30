using MessageBus;
using Microsoft.EntityFrameworkCore;
using Tasks;
using Tasks.Application.IntegrationEvents;
using Tasks.Infrastructure.DataAccess;
using Tasks.Middleware;

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
app.UseCors("allowAny");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
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
    await Task.CompletedTask;
    return;
}
async Task AddSubscriptions(WebApplication app)
{
    var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

    var subscribeTasks = new List<Task>
    {
        eventBus.Subscribe<ProjectMemberAddedEvent>(),
        eventBus.Subscribe<ProjectMemberUpdatedEvent>(),
        eventBus.Subscribe<ProjectMemberRemovedEvent>(),
        eventBus.Subscribe<ProjectRemovedEvent>(),
        eventBus.Subscribe<ProjectMemberAcceptInvitationEvent>(),
        eventBus.Subscribe<ProjectMemberDeclineInvitationEvent>(),
    };

    await Task.WhenAll(subscribeTasks);
}

public partial class Program { }