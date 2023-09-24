namespace Projects.Domain.AggregatesModel.ProjectAggregate;

public interface IProjectRepository
{
    Task<Project?> GetOneAsync(string projectId);
    Task<int> ExecuteDeleteAsync(string id);
    void Add(Project entity);
    void Remove(Project entity);
}
