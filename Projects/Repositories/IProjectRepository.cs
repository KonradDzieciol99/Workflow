using Projects.Entity;

namespace Projects.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetOneAsync(string projectId);
    }
}
