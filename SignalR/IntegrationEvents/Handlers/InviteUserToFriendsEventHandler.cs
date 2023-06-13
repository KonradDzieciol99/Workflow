using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

public class InviteUserToFriendsEventHandler : IRequestHandler<FriendRequestAddedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public InviteUserToFriendsEventHandler(IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(FriendRequestAddedEvent request, CancellationToken cancellationToken)
    {
        //await _presenceHubContext.Clients.User(request.InvitedUser.UserId).SendAsync("NewInvitationToFriendsReceived", new { inviterEmail = request.UserWhoInvited.UserEmail });
        await _messagesHubContext.Clients.User(request.InvitedUserId).SendAsync("NewInvitationToFriendsReceived", request);
    }
}
