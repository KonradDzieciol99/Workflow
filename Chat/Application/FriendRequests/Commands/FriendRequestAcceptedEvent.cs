using MessageBus;

namespace Chat.Application.FriendRequests.Commands;
internal class FriendRequestAcceptedEvent : IntegrationEvent
{
    private string inviterUserId;
    private string inviterUserEmail;
    private string? inviterPhotoUrl;
    private string invitedUserId;
    private string invitedUserEmail;
    private string? invitedPhotoUrl;

    public FriendRequestAcceptedEvent(string inviterUserId, string inviterUserEmail, string? inviterPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedPhotoUrl)
    {
        this.inviterUserId = inviterUserId;
        this.inviterUserEmail = inviterUserEmail;
        this.inviterPhotoUrl = inviterPhotoUrl;
        this.invitedUserId = invitedUserId;
        this.invitedUserEmail = invitedUserEmail;
        this.invitedPhotoUrl = invitedPhotoUrl;
    }
}