using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Application.Common.Interfaces;

public interface IReadOnlyProjectRepository
{
    Task<Project?> GetOneAsync(string projectId);
}