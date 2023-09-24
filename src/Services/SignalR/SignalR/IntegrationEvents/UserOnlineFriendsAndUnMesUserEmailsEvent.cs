using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public record UserOnlineFriendsAndUnMesUserEmailsEvent(
    UserDto OnlineUser,
    List<UserDto> ListOfAcceptedFriends,
    List<string> UnreadMessagesUserEmails
) : IntegrationEvent;

public class UserOnlineFriendsAndUnMesUserEmailsEventHandler
    : IRequestHandler<UserOnlineFriendsAndUnMesUserEmailsEvent>
{
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly IDatabase _redisDb;

    public UserOnlineFriendsAndUnMesUserEmailsEventHandler(
        IConnectionMultiplexer connectionMultiplexer,
        IHubContext<PresenceHub> presenceHubContext
    )
    {
        _presenceHubContext =
            presenceHubContext ?? throw new ArgumentNullException(nameof(presenceHubContext));
        _redisDb =
            connectionMultiplexer.GetDatabase()
            ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task Handle(
        UserOnlineFriendsAndUnMesUserEmailsEvent request,
        CancellationToken cancellationToken
    )
    {
        var onlineStatusTasks = request.ListOfAcceptedFriends
            .Select(friend => _redisDb.KeyExistsAsync($"presence-{friend.Email}"))
            .ToList();

        var onlineStatuses = await Task.WhenAll(onlineStatusTasks);

        var onlineUsers = request.ListOfAcceptedFriends
            .Where((friend, i) => onlineStatuses[i])
            .ToList();

        await _presenceHubContext.Clients
            .Users(request.ListOfAcceptedFriends.Select(x => x.Id))
            .SendAsync(
                "UserIsOnline",
                request.OnlineUser.Email,
                cancellationToken: cancellationToken
            );
        await _presenceHubContext.Clients
            .User(request.OnlineUser.Id)
            .SendAsync(
                "ReceiveOnlineUsers",
                onlineUsers.Select(x => x.Email),
                cancellationToken: cancellationToken
            );
        await _presenceHubContext.Clients
            .User(request.OnlineUser.Id)
            .SendAsync(
                "ReceiveUnreadMessages",
                request.UnreadMessagesUserEmails,
                cancellationToken: cancellationToken
            );

        return;
    }
}
