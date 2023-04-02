using Projects.Entity;
using Projects.Models;

namespace Projects.Repositories
{
    public interface IProjectMemberRepository : IRepository<ProjectMember>
    {
        Task<List<Project>> GetUserProjects(string userId, AppParams appParams);
    }
}
