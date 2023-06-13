using MessageBus.Events;
using MessageBus;
using SignalR;
using SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

var subscribeTasks = new List<Task>
{
    eventBus.Subscribe<ChatMessageAddedEvent>(),
    eventBus.Subscribe<NewOnlineUserWithFriendsEvent>(),
    eventBus.Subscribe<NewOnlineMessagesUserWithFriendsEvent>(),
    eventBus.Subscribe<NewOfflineUserWithFriendsEvent>(),
    eventBus.Subscribe<FriendRequestAcceptedEvent>(),
    eventBus.Subscribe<FriendRequestAddedEvent>(),
    eventBus.Subscribe<NotificationAddedEvent>(),
};

await Task.WhenAll(subscribeTasks);

if (app.Environment.IsDevelopment()){}

app.UseHttpsRedirection();

app.UseCors("allowAny");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/hub/Chat");

app.MapHub<PresenceHub>("/hub/Presence");

app.MapHub<MessagesHub>("/hub/Messages");

await app.RunAsync();

