using Microsoft.EntityFrameworkCore;
using Projects.Domain.Entities;
using Projects.Domain.Interfaces;
using Projects.Infrastructure.DataAccess;

namespace Projects.Infrastructure.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProjectRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Project?> GetOneAsync(string projectId)
        {
            return await _applicationDbContext.Projects.Include(x => x.ProjectMembers).SingleOrDefaultAsync(x => x.Id == projectId);
        }
        public async Task<int> ExecuteDeleteAsync(string id)
        {
            return await _applicationDbContext.Projects.Where(x => x.Id == id).ExecuteDeleteAsync();
        }


    }
}
