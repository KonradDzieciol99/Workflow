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
        //public async Task<Project> GetOneAsync(string projectName,string userId)
        //{
        //    //return await _applicationDbContext.Projects.Where(x=>x.Name==projectName);
        //}
    }
}
