using Tasks.Entity;

namespace Tasks.Repositories
{
    public interface IProjectMemberRepository : IRepository<ProjectMember>
    {
        Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId);
        Task<int> RemoveAsync(string projectId, string userId);
        Task<int> UpdateAsync(string projectId, string userId, ProjectMemberType projectMemberType);
        Task<int> RemoveAllProjectMembersAsync(string projectId);
    }
}
