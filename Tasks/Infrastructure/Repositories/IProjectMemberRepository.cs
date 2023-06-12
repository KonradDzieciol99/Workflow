using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;

namespace Tasks.Infrastructure.Repositories
{
    public interface IProjectMemberRepository : IRepository<ProjectMember>
    {
        Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId);
        Task<int> RemoveAsync(string projectMemberId);
        Task<int> UpdateAsync(string projectMemberId, ProjectMemberType projectMemberType);
        Task<int> RemoveAllProjectMembersAsync(string projectId);
        Task<ProjectMember?> GetAsync(string Id);
        Task<ProjectMember?> GetAsync(string userId, string projectId);
    }
}
