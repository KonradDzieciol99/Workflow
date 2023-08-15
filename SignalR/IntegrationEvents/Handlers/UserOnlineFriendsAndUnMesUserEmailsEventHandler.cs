using MediatR;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents.Handlers;

public class UserOnlineFriendsAndUnMesUserEmailsEventHandler : IRequestHandler<UserOnlineFriendsAndUnMesUserEmailsEvent>
{

    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly IDatabase _redisDb;

    public UserOnlineFriendsAndUnMesUserEmailsEventHandler(IConnectionMultiplexer connectionMultiplexer,
        IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _presenceHubContext = presenceHubContext;
        _redisDb = connectionMultiplexer.GetDatabase();
    }
    public async Task Handle(UserOnlineFriendsAndUnMesUserEmailsEvent request, CancellationToken cancellationToken)
    {

        var onlineStatusTasks = request.ListOfAcceptedFriends
            .Select(friend => _redisDb.KeyExistsAsync($"presence-{friend.Email}"))
            .ToList();

        var onlineStatuses = await Task.WhenAll(onlineStatusTasks);

        var onlineUsers = request.ListOfAcceptedFriends
            .Where((friend, i) => onlineStatuses[i])
            .ToList();

        await _presenceHubContext.Clients.Users(request.ListOfAcceptedFriends.Select(x => x.Id)).SendAsync("UserIsOnline", request.OnlineUser.Email); 
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveOnlineUsers", onlineUsers.Select(x=>x.Email));
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveUnreadMessages", request.UnreadMessagesUserEmails);

        return;
    }
}
