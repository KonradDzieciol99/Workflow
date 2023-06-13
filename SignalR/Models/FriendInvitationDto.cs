namespace SignalR.Models;

public class FriendInvitationDto
{
    public string InviterUserId { get; set; }
    public string InviterUserEmail { get; set; }
    public string? InviterPhotoUrl { get; set; }
    public string InvitedUserId { get; set; }
    public string InvitedUserEmail { get; set; }
    public string? InvitedPhotoUrl { get; set; }
    public bool Confirmed { get; set; }
}
