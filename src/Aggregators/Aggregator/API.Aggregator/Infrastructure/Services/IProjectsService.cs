using API.Aggregator.Application.Commons.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IProjectsService
{
    Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId);
    Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId);
    Task<ProjectMemberDto?> AddMember(string projectId, object command);
}
