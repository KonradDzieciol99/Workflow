namespace SignalR.Models;

public class FriendInvitationDto
{
    public FriendInvitationDto()
    {
        
    }

    public FriendInvitationDto(string inviterUserId, string inviterUserEmail, string? inviterPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedPhotoUrl, bool confirmed)
    {
        InviterUserId = inviterUserId ?? throw new ArgumentNullException(nameof(inviterUserId));
        InviterUserEmail = inviterUserEmail ?? throw new ArgumentNullException(nameof(inviterUserEmail));
        InviterPhotoUrl = inviterPhotoUrl;
        InvitedUserId = invitedUserId ?? throw new ArgumentNullException(nameof(invitedUserId));
        InvitedUserEmail = invitedUserEmail ?? throw new ArgumentNullException(nameof(invitedUserEmail));
        InvitedPhotoUrl = invitedPhotoUrl;
        Confirmed = confirmed;
    }

    public string InviterUserId { get; set; }
    public string InviterUserEmail { get; set; }
    public string? InviterPhotoUrl { get; set; }
    public string InvitedUserId { get; set; }
    public string InvitedUserEmail { get; set; }
    public string? InvitedPhotoUrl { get; set; }
    public bool Confirmed { get; set; }
}
