using Chat.Domain.Common.Exceptions;
using Chat.Domain.Common.Models;

namespace Chat.Domain.Entity;

public class FriendRequest : BaseEntityWithCompositeKey
{
    public FriendRequest() { }
    public FriendRequest(string inviterUserId, string inviterUserEmail, string? inviterPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedPhotoUrl)
    {
        InviterUserId = inviterUserId ?? throw new ArgumentNullException(nameof(inviterUserId));
        InviterUserEmail = inviterUserEmail ?? throw new ArgumentNullException(nameof(inviterUserEmail));
        InviterPhotoUrl = inviterPhotoUrl;
        InvitedUserId = invitedUserId ?? throw new ArgumentNullException(nameof(invitedUserId));
        InvitedUserEmail = invitedUserEmail ?? throw new ArgumentNullException(nameof(invitedUserEmail));
        InvitedPhotoUrl = invitedPhotoUrl;
        Confirmed = false;
    }
    public string InviterUserId { get; private set; }
    public string InviterUserEmail { get; private set; }
    public string? InviterPhotoUrl { get; private set; }
    public string InvitedUserId { get; private set; }
    public string InvitedUserEmail { get; private set; }
    public string? InvitedPhotoUrl { get; private set; }
    public bool Confirmed { get; private set; } = false;
    public void AcceptRequest(string currentUserId)
    {
        if (Confirmed)
            throw new ChatDomainException("Invitation is already confirmed.");

        if (currentUserId == InviterUserId)
            throw new ChatDomainException("You cannot confirm your own Friend Request.");

        Confirmed = true;
    }
}
