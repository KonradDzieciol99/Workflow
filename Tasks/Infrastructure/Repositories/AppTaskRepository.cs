using Microsoft.EntityFrameworkCore;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.DataAccess;
using Tasks.Models;

namespace Tasks.Infrastructure.Repositories
{
    public class AppTaskRepository : Repository<AppTask>, IAppTaskRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AppTaskRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<bool> CheckIfUserHasRightsToMenageTaskAsync(string projectId, string userId, string appTaskId)
        {
            return await _applicationDbContext.ProjectMembers.AnyAsync(x => x.UserId == userId && x.ProjectId == projectId
                                                                    && ((x.Type == ProjectMemberType.Admin || x.Type == ProjectMemberType.Leader)
                                                                    || (x.ConductedTasks.Any(x => x.Id == appTaskId))));
        }

        public async Task<AppTask?> Get(string Id)
        {
            return await _applicationDbContext.AppTasks.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<AppTask>> GetAllProjectTasksAsync(string projectId)
        {
            return await _applicationDbContext.AppTasks.Where(x => x.ProjectId == projectId)
                                                       .ToListAsync();
        }
    }
}
