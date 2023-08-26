using MessageBus;

namespace Chat.Application.IntegrationEvents;

public class FriendRequestCanceledEvent : IntegrationEvent
{
    public FriendRequestCanceledEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string declinedInvitationUserId, string declinedInvitationUserEmail, string? declinedInvitationUserPhotoUrl)
    {
        InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
        InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
        InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
        DeclinedInvitationUserId = declinedInvitationUserId ?? throw new ArgumentNullException(nameof(declinedInvitationUserId));
        DeclinedInvitationUserEmail = declinedInvitationUserEmail ?? throw new ArgumentNullException(nameof(declinedInvitationUserEmail));
        DeclinedInvitationUserPhotoUrl = declinedInvitationUserPhotoUrl;
    }

    public string InvitationSendingUserId { get; set; }
    public string InvitationSendingUserEmail { get; set; }
    public string? InvitationSendingUserPhotoUrl { get; set; }

    public string DeclinedInvitationUserId { get; set; }
    public string DeclinedInvitationUserEmail { get; set; }
    public string? DeclinedInvitationUserPhotoUrl { get; set; }
}
