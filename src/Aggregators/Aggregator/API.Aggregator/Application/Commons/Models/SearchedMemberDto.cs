namespace API.Aggregator.Application.Commons.Models;

public record SearchedMemberDto(string Id, string Email, string? PhotoUrl, MemberStatusType Status);
