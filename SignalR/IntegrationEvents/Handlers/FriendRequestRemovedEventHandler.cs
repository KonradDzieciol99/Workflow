using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public FriendRequestRemovedEventHandler(IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        //await _presenceHubContext.Clients.Users(request.FriendToRemoveUserId).SendAsync("UserIsOffline", request.ActionInitiatorUserEmail);
        await _messagesHubContext.Clients.Users(request.FriendToRemoveUserId).SendAsync("FriendRequestRemoved", request.ActionInitiatorUserEmail);
        return;
    }
}
