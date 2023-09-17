using System.Diagnostics.CodeAnalysis;

namespace SignalR.Commons.Models;

public class FriendInvitationDto
{
    public FriendInvitationDto()
    {

    }
    [SetsRequiredMembers]
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

    public required string InviterUserId { get; set; }
    public required string InviterUserEmail { get; set; }
    public string? InviterPhotoUrl { get; set; }
    public required string InvitedUserId { get; set; }
    public required string InvitedUserEmail { get; set; }
    public string? InvitedPhotoUrl { get; set; }
    public required bool Confirmed { get; set; }
}
