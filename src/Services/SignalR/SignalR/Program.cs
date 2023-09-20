using HealthChecks.UI.Client;
using Logging;
using MessageBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using SignalR.Hubs;
using SignalR.IntegrationEvents;
using System.Net;

namespace SignalR;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseSerilog(SeriLogger.Configure);

        builder.Services.AddWebAPIServices(builder.Configuration);

        var app = builder.Build();

        await AddSubscriptions(app);

        if (app.Environment.IsDevelopment()) { }

        app.UseCors("allowAny");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHub<ChatHub>("/hub/Chat");
        app.MapHub<PresenceHub>("/hub/Presence");
        app.MapHub<MessagesHub>("/hub/Messages");
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

        static async Task AddSubscriptions(WebApplication app)
        {
            var eventBus = app.Services.GetRequiredService<IEventBusConsumer>();

            var subscribeTasks = new List<Task>
        {
            eventBus.Subscribe<ChatMessageAddedEvent>(),
            eventBus.Subscribe<UserOnlineFriendsAndUnMesUserEmailsEvent>(),
            eventBus.Subscribe<NewOnlineMessagesUserWithFriendsEvent>(),
            eventBus.Subscribe<UserOfflineWithFriendsEvent>(),
            eventBus.Subscribe<FriendRequestAcceptedEvent>(),
            eventBus.Subscribe<FriendRequestAddedEvent>(),
            eventBus.Subscribe<NotificationAddedEvent>(),
            eventBus.Subscribe<UserOnlineNotifcationsAndUnreadEvent>(),
            eventBus.Subscribe<FriendRequestRemovedEvent>(),
            eventBus.Subscribe<UserConnectedToChatResponseEvent>(),
        };

            await Task.WhenAll(subscribeTasks);
        }
    }
}