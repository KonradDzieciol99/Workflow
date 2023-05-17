using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.Interfaces;

public interface IReadOnlyProjectRepository
{
    Task<Project?> GetOneAsync(string projectId);
}