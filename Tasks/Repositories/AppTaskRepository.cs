using Tasks.DataAccess;
using Tasks.Entity;

namespace Tasks.Repositories
{
    public class AppTaskRepository : Repository<AppTask>, IAppTaskRepository
    {
        public AppTaskRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
