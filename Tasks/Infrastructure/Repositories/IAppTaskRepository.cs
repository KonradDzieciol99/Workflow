using Tasks.Domain.Entity;

namespace Tasks.Infrastructure.Repositories
{
    public interface IAppTaskRepository : IRepository<AppTask>
    {
        Task<bool> CheckIfUserHasRightsToMenageTaskAsync(string projectId, string userId, string appTaskId);
        Task<List<AppTask>> GetAllProjectTasksAsync(string projectId);
        Task<AppTask?> Get(string Id);
    }
}
