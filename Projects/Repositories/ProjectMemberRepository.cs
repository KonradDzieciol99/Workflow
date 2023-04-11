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
        //public async Task<List<Project>> GetUserProjects(string userId,AppParams appParams)
        public async Task<(List<Project> Projects, int TotalCount)> GetUserProjects(string userId,AppParams appParams)
        {

            var query = applicationDbContext.ProjectMembers.AsQueryable();

            query = query.Include(pm => pm.MotherProject)
                         .ThenInclude(p => p.ProjectMembers);

            if (string.IsNullOrWhiteSpace(appParams.OrderBy) == false && appParams.IsDescending.HasValue)
            {
                query.OrderBy(appParams.OrderBy, appParams.IsDescending.Value);
            }

            query = string.IsNullOrWhiteSpace(appParams.Search) switch
            {
                true => query.Where(x => x.UserId == userId), 
                false => query.Where(x => x.UserId == userId && x.MotherProject.Name.StartsWith(appParams.Search)),
            };

            int totalCount = await query.CountAsync();

            var projects = await query.Select(pm => pm.MotherProject)
                              .Skip(appParams.Skip)
                              .Take(appParams.Take)
                              .ToListAsync();

            return (projects, totalCount);
        }
    }
}
