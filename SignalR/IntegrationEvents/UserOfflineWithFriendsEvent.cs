using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public class UserOfflineWithFriendsEvent : IntegrationEvent
{
    public UserOfflineWithFriendsEvent(UserDto user, IEnumerable<UserDto> userChatFriends)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserChatFriends = userChatFriends ?? throw new ArgumentNullException(nameof(userChatFriends));
    }

    public UserDto User { get; set; }
    public IEnumerable<UserDto> UserChatFriends { get; set; }
}
public class UserOfflineWithFriendsEventHandler : IRequestHandler<UserOfflineWithFriendsEvent>
{
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public UserOfflineWithFriendsEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        this._presenceHubContext = presenceHubContext;
    }
    public async Task Handle(UserOfflineWithFriendsEvent request, CancellationToken cancellationToken)
    {
        //await _messagesHubContext.Clients.Users(request.UserChatFriends.Select(x => x.Id)).SendAsync("UserIsOffline", request.User);
        await _presenceHubContext.Clients.Users(request.UserChatFriends.Select(x => x.Id)).SendAsync("UserIsOffline", request.User.Email);
        return;
    }
}
