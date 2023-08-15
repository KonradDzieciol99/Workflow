using Microsoft.EntityFrameworkCore;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Infrastructure.DataAccess;

namespace Projects.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProjectRepository(ApplicationDbContext applicationDbContext) 
        {
            _applicationDbContext = applicationDbContext  ?? throw new ArgumentNullException(nameof(_applicationDbContext));
        }
        public async Task<Project?> GetOneAsync(string projectId)
        {
            return await _applicationDbContext.Projects.Include(x => x.ProjectMembers)
                                                       .SingleOrDefaultAsync(x => x.Id == projectId);
        }
        public async Task<int> ExecuteDeleteAsync(string id)
        {
            return await _applicationDbContext.Projects.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
        private readonly ApplicationDbContext _dbContext;

        public void Add(Project entity)
        {
            _applicationDbContext.Projects.Add(entity);
        }
        public void Remove(Project entity)
        {
            _applicationDbContext.Projects.Remove(entity);
        }
    }
}
