using Microsoft.EntityFrameworkCore;
using Tasks.DataAccess;
using Tasks.Entity;

namespace Tasks.Repositories
{
    public class AppTaskRepository : Repository<AppTask>, IAppTaskRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AppTaskRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public async Task<List<AppTask>> GetAllProjectTasksAsync(string projectId)
        {
            return await _applicationDbContext.AppTasks.Where(x => x.ProjectId == projectId)
                                                       .ToListAsync();
        }
    }
}
