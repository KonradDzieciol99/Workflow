using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public record UserOfflineWithFriendsEvent(UserDto User, IEnumerable<UserDto> UserChatFriends)
    : IntegrationEvent;

public class UserOfflineWithFriendsEventHandler : IRequestHandler<UserOfflineWithFriendsEvent>
{
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public UserOfflineWithFriendsEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        this._presenceHubContext =
            presenceHubContext ?? throw new ArgumentNullException(nameof(presenceHubContext));
    }

    public async Task Handle(
        UserOfflineWithFriendsEvent request,
        CancellationToken cancellationToken
    )
    {
        await _presenceHubContext.Clients
            .Users(request.UserChatFriends.Select(x => x.Id))
            .SendAsync("UserIsOffline", request.User.Email, cancellationToken: cancellationToken);
        return;
    }
}
