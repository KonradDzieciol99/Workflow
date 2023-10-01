using API.Aggregator.Application.Common.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IProjectsService
{
    Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId, CancellationToken cancellationToken);
    Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId, CancellationToken cancellationToken);
    Task<ProjectMemberDto?> AddMember(string projectId, object command, CancellationToken cancellationToken);
}
