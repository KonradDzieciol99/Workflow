using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents.Handlers;

public class NewOfflineUserWithFriendsEventHandler : IRequestHandler<NewOfflineUserWithFriendsEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private IDatabase _redisDb;

    public NewOfflineUserWithFriendsEventHandler(IConnectionMultiplexer connectionMultiplexer,
        IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _redisDb = connectionMultiplexer.GetDatabase();
    }
    public async Task Handle(NewOfflineUserWithFriendsEvent request, CancellationToken cancellationToken)
    {
        await _messagesHubContext.Clients.Users(request.UserChatFriends.Select(x => x.UserId)).SendAsync("UserIsOffline", request.User);
        return;
    }
}
