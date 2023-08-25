using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

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
