using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Events.Handlers
{
    public class InviteUserToFriendsEventHandler : IRequestHandler<FriendInvitationAddedEvent>
    {
        private readonly IHubContext<MessagesHub> _messagesHubContext;
        private readonly IHubContext<PresenceHub> _presenceHubContext;

        public InviteUserToFriendsEventHandler(IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
        {
            this._messagesHubContext = messagesHubContext;
            this._presenceHubContext = presenceHubContext;
        }
        public async Task Handle(FriendInvitationAddedEvent request, CancellationToken cancellationToken)
        {
            //await _presenceHubContext.Clients.User(request.InvitedUser.UserId).SendAsync("NewInvitationToFriendsReceived", new { inviterEmail = request.UserWhoInvited.UserEmail });
            await _messagesHubContext.Clients.User(request.InvitedUserId).SendAsync("NewInvitationToFriendsReceived", request);
        }
    }
}
