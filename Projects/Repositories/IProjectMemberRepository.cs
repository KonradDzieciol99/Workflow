using Projects.Entity;
using Projects.Models;

namespace Projects.Repositories
{
    public interface IProjectMemberRepository : IRepository<ProjectMember>
    {
        //Task<List<Project>> GetUserProjects(string userId, AppParams appParams);
        Task<(List<Project> Projects, int TotalCount)> GetUserProjects(string userId, AppParams appParams);
        Task<Project?> GetOneAsync(string projectName, string userId);
        Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId);
        Task<bool> CheckIfUserHasRightsToMenageUserAsync(string projectId, string userId);
        Task<int> RemoveAsync(string projectId, string userId);
        Task<ProjectMember?> GetProjectMemberAsync(string projectId, string userId);
        Task<bool> CheckIfUserIsALeaderAsync(string projectId, string userId);

    }
}
