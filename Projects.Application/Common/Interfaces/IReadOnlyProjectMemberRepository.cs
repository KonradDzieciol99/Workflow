using Projects.Application.Common.Models;
using Projects.Application.Projects.Queries;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Models;

namespace Projects.Application.Common.Interfaces
{
    public interface IReadOnlyProjectMemberRepository
    {
        Task<(List<Project> Projects, int TotalCount)> Get(string userId, GetProjectsQuery appParams);
        Task<Project?> GetOneAsync(string projectName, string userId);
        Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId);
        Task<bool> CheckIfUserHasRightsToMenageUserAsync(string projectId, string userId);
        Task<ProjectMember?> GetProjectMemberAsync(string projectId, string userId);
        Task<bool> CheckIfUserIsALeaderAsync(string projectId, string userId);
        Task<ProjectMember?> GetAsync(string projectMemberId);

    }
}
