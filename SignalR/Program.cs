using MessageBus;
using SignalR;
using SignalR.Hubs;
using SignalR.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);

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
app.Run();


async Task AddSubscriptions(WebApplication app)
{
    var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

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
