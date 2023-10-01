namespace API.Aggregator.Application.Common.Models;

public record SearchedMemberDto(string Id, string Email, string? PhotoUrl, MemberStatusType Status);
