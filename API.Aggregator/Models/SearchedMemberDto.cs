namespace API.Aggregator.Models;

public record SearchedMemberDto(string Id, string Email, string? PhotoUrl, MemberStatusType Status);
