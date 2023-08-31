using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public class UserOnlineFriendsAndUnMesUserEmailsEvent : IntegrationEvent
{
    public UserOnlineFriendsAndUnMesUserEmailsEvent(UserDto onlineUser, List<UserDto> listOfAcceptedFriends, List<string> unreadMessagesUserEmails)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
        ListOfAcceptedFriends = listOfAcceptedFriends ?? throw new ArgumentNullException(nameof(listOfAcceptedFriends));
        UnreadMessagesUserEmails = unreadMessagesUserEmails ?? throw new ArgumentNullException(nameof(unreadMessagesUserEmails));
    }

    public UserDto OnlineUser { get; set; }
    public List<UserDto> ListOfAcceptedFriends { get; set; }
    public List<string> UnreadMessagesUserEmails { get; set; }
}
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
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveOnlineUsers", onlineUsers.Select(x => x.Email));
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveUnreadMessages", request.UnreadMessagesUserEmails);

        return;
    }
}
