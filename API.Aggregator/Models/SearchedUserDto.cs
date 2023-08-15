namespace API.Aggregator.Models;

public record SearchedUserDto(string Id, string Email, string? PhotoUrl, FriendStatusType Status);
