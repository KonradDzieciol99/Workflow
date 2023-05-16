using Microsoft.EntityFrameworkCore;
using Projects.DataAccess;
using Projects.Entity;

namespace Projects.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProjectRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public async Task<Project?> GetOneAsync(string projectId)
        {
            return await _applicationDbContext.Projects.Include(x=>x.ProjectMembers).SingleOrDefaultAsync(x => x.Id == projectId);
        }
    }
}
