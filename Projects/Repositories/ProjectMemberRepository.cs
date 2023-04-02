using Microsoft.EntityFrameworkCore;
using Projects.DataAccess;
using Projects.Entity;
using Projects.Models;
using System.Linq;
using System.Linq.Expressions;
using Projects.Extensions;

namespace Projects.Repositories
{
    public class ProjectMemberRepository : Repository<ProjectMember>, IProjectMemberRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ProjectMemberRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        //public async Task<List<Project>> GetUserProjects(string userId)
        //{
        //    return await applicationDbContext.ProjectMembers.Where(pm => pm.UserId == userId)
        //                                                    .Select(pm => pm.Project)
        //                                                    .ToListAsync();
        //}
        public async Task<List<Project>> GetUserProjects(string userId,AppParams appParams)
        {

            var query = applicationDbContext.ProjectMembers.AsQueryable();

            query = query.Skip(appParams.Skip)
                         .Take(appParams.Take)
                         .Where(pm => pm.UserId == userId);

            if (string.IsNullOrWhiteSpace(appParams.OrderBy) == false && appParams.IsDescending.HasValue)
            {
                query.OrderBy(appParams.OrderBy, appParams.IsDescending.Value);
            }

            if (string.IsNullOrWhiteSpace(appParams.Search) == false)
            {
                query.Where(x => x.Project.Name.StartsWith(appParams.Search));
            }

            return await query.Select(pm => pm.Project)
                              .ToListAsync();
        }
    }
}
