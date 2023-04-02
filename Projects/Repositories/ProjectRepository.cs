using Projects.DataAccess;
using Projects.Entity;

namespace Projects.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
