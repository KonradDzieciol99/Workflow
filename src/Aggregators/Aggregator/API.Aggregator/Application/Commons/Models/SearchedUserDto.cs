namespace API.Aggregator.Application.Commons.Models;

public record SearchedUserDto(string Id, string Email, string? PhotoUrl, FriendStatusType Status);
