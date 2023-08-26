using MessageBus;

namespace Notification.Application.IntegrationEvents;

public class FriendRequestAcceptedEvent : IntegrationEvent
{
    public FriendRequestAcceptedEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string invitationAcceptingUserId, string invitationAcceptingUserEmail, string? invitationAcceptingUserPhotoUrl)
    {
        InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
        InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
        InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
        InvitationAcceptingUserId = invitationAcceptingUserId ?? throw new ArgumentNullException(nameof(invitationAcceptingUserId));
        InvitationAcceptingUserEmail = invitationAcceptingUserEmail ?? throw new ArgumentNullException(nameof(invitationAcceptingUserEmail));
        InvitationAcceptingUserPhotoUrl = invitationAcceptingUserPhotoUrl;
    }

    public string InvitationSendingUserId { get; set; }
    public string InvitationSendingUserEmail { get; set; }
    public string? InvitationSendingUserPhotoUrl { get; set; }

    public string InvitationAcceptingUserId { get; set; }
    public string InvitationAcceptingUserEmail { get; set; }
    public string? InvitationAcceptingUserPhotoUrl { get; set; }
}
