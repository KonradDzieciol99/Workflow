using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;

    public FriendRequestRemovedEventHandler(IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext;
    }
    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        await _messagesHubContext.Clients.Users(request.FriendToRemoveUserId).SendAsync("FriendRequestRemoved", request.ActionInitiatorUserEmail);
        return;
    }
}
