using HealthChecks.UI.Client;
using HttpMessage.Middleware;
using Logging;
using MessageBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Common.Exceptions;
using Tasks.Infrastructure.DataAccess;

namespace Tasks;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services.AddTasksServices(builder.Configuration);

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
        app.UseMiddleware<ExceptionMiddleware<TaskDomainException>>();
        app.MapDefaultControllerRoute();
        app.MapControllers();
        app.MapHealthChecks(
            "/hc",
            new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }
        );
        app.MapHealthChecks(
            "/liveness",
            new HealthCheckOptions { Predicate = r => r.Name.Contains("self") }
        );
        app.Run();

        async Task ApplyMigration()
        {
            using var scope = app.Services.CreateScope();
            var initialiser = scope.ServiceProvider.GetRequiredService<SeedData>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
            return;
        }
        async Task AddSubscriptions(WebApplication app)
        {
            var eventBus = app.Services.GetRequiredService<IEventBusConsumer>();

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
    }
}
