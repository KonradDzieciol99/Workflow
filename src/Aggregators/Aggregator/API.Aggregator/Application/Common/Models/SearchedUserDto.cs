namespace API.Aggregator.Application.Common.Models;

public record SearchedUserDto(string Id, string Email, string? PhotoUrl, FriendStatusType Status);
