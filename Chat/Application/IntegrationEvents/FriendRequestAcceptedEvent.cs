using MessageBus;

namespace Chat.Application.IntegrationEvents;
internal class FriendRequestAcceptedEvent : IntegrationEvent
{

    public FriendRequestAcceptedEvent(string inviterUserId, string inviterUserEmail, string? inviterPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedPhotoUrl)
    {
        this.inviterUserId = inviterUserId;
        this.inviterUserEmail = inviterUserEmail;
        this.inviterPhotoUrl = inviterPhotoUrl;
        this.invitedUserId = invitedUserId;
        this.invitedUserEmail = invitedUserEmail;
        this.invitedPhotoUrl = invitedPhotoUrl;
    }

    public string inviterUserId { get; private set; }
    public string inviterUserEmail { get; private set; }
    public string? inviterPhotoUrl { get; private set; }
    public string invitedUserId { get; private set; }
    public string invitedUserEmail { get; private set; }
    public string? invitedPhotoUrl { get; private set; }
}