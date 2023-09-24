using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record FriendRequestDeclinedEvent(
    string InvitationSendingUserId,
    string InvitationSendingUserEmail,
    string? InvitationSendingUserPhotoUrl,
    string DeclinedInvitationUserId,
    string DeclinedInvitationUserEmail,
    string? DeclinedInvitationUserPhotoUrl
) : IntegrationEvent;
