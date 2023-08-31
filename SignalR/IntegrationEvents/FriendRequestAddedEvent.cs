using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;

namespace SignalR.IntegrationEvents;

public class FriendRequestAddedEvent : IntegrationEvent
{
    public FriendRequestAddedEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedUserPhotoUrl)
    {
        InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
        InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
        InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
        InvitedUserId = invitedUserId ?? throw new ArgumentNullException(nameof(invitedUserId));
        InvitedUserEmail = invitedUserEmail ?? throw new ArgumentNullException(nameof(invitedUserEmail));
        InvitedUserPhotoUrl = invitedUserPhotoUrl;
    }

    public string InvitationSendingUserId { get; set; }
    public string InvitationSendingUserEmail { get; set; }
    public string? InvitationSendingUserPhotoUrl { get; set; }

    public string InvitedUserId { get; set; }
    public string InvitedUserEmail { get; set; }
    public string? InvitedUserPhotoUrl { get; set; }

}
public class FriendRequestAddedEventHandler : IRequestHandler<FriendRequestAddedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public FriendRequestAddedEventHandler(IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(FriendRequestAddedEvent request, CancellationToken cancellationToken)
    {
        //await _presenceHubContext.Clients.User(request.InvitedUser.UserId).SendAsync("NewInvitationToFriendsReceived", new { inviterEmail = request.UserWhoInvited.UserEmail });
        await _messagesHubContext.Clients.User(request.InvitedUserId).SendAsync(
            "NewInvitationToFriendsReceived",
            new FriendInvitationDto(request.InvitationSendingUserId,
                                    request.InvitationSendingUserEmail,
                                    request.InvitationSendingUserPhotoUrl,
                                    request.InvitedUserId,
                                    request.InvitedUserEmail,
                                    request.InvitedUserPhotoUrl,
                                    false));
    }
}
