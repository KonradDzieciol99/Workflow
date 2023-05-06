using Tasks.Entity;

namespace Tasks.Repositories
{
    public interface IAppTaskRepository : IRepository<AppTask>
    {
        Task<List<AppTask>> GetAllProjectTasksAsync(string projectId);
    }
}
