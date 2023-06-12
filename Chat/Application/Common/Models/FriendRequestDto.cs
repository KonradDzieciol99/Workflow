namespace Chat.Application.Common.Models;

public record FriendRequestDto(string InviterUserId,
                               string InviterUserEmail,
                               string? InviterPhotoUrl,
                               string InvitedUserId,
                               string InvitedUserEmail,
                               string? InvitedPhotoUrl,
                               bool Confirmed);

