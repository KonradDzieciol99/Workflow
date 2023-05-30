using Microsoft.EntityFrameworkCore;
using MessageBus.Events;
using MessageBus;
using Tasks.Infrastructure.DataAccess;
using Tasks.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();// nie potrzeba tworzyæ scope bo to singletone

var subscribeTasks = new List<Task>
{
    eventBus.Subscribe<ProjectMemberAddedEvent>(),
    eventBus.Subscribe<ProjectMemberUpdatedEvent>(),
    eventBus.Subscribe<ProjectMemberRemovedEvent>(),
    eventBus.Subscribe<ProjectRemovedEvent>(),
};
await Task.WhenAll(subscribeTasks);

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
