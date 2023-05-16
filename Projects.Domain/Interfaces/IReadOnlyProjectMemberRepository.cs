using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Models;

namespace Projects.Domain.Interfaces
{
    public interface IReadOnlyProjectMemberRepository //: IReadOnlyRepository<ProjectMember>
    {
        Task<(List<Project> Projects, int TotalCount)> GetUserProjects(string userId, AppParams appParams);
        Task<Project?> GetOneAsync(string projectName, string userId);
        Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId);
        Task<bool> CheckIfUserHasRightsToMenageUserAsync(string projectId, string userId);
        Task<ProjectMember?> GetProjectMemberAsync(string projectId, string userId);
        Task<bool> CheckIfUserIsALeaderAsync(string projectId, string userId);
        Task<ProjectMember?> GetAsync(string projectMemberId);

    }
}
