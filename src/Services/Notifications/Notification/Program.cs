using HealthChecks.UI.Client;
using HttpMessage.Middleware;
using Logging;
using MessageBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Notification.Application.IntegrationEvents;
using Notification.Domain.Common.Exceptions;
using Notification.Infrastructure.DataAccess;
using Serilog;

namespace Notification;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

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
        app.UseMiddleware<ExceptionMiddleware<NotificationDomainException>>();
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
        async Task AddSubscriptions(WebApplication app)
        {
            var eventBus = app.Services.GetRequiredService<IEventBusConsumer>();

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
        }
    }
}
