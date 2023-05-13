using Projects.Domain.Entities;

namespace Projects.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetOneAsync(string projectId);
        Task<int> ExecuteDeleteAsync(string id);
    }
}
