using Tasks.Domain.Common.Models;

namespace Tasks.Application.Common.Models;

public record ProjectMemberDto(string Id, string UserId, string UserEmail, string? PhotoUrl, ProjectMemberType Type, string ProjectId);

