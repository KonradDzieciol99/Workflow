using Tasks.Application.AppTasks.Queries;
using Tasks.Domain.Entity;

namespace Tasks.Infrastructure.Repositories;

public interface IAppTaskRepository : IRepository<AppTask>
{
    Task<bool> CheckIfUserHasRightsToMenageTaskAsync(string projectId, string userId, string appTaskId);
    Task<List<AppTask>> GetAllProjectTasksAsync(string projectId);
    Task<AppTask?> GetAsync(string Id);
    Task<(List<AppTask> appTasks, int totalCount)> GetAsync(string userId, GetAppTasksQuery query);

}
