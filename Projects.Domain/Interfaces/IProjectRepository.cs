using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.Interfaces
{
    public interface IProjectRepository 
    {

        Task<Project?> GetOneAsync(string projectId);
        Task<int> ExecuteDeleteAsync(string id);
        void Add(Project entity);
        void Remove(Project entity);

    }
}
