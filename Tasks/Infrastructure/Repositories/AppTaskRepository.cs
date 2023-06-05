using Microsoft.EntityFrameworkCore;
using Tasks.Application.AppTasks.Queries;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.DataAccess;
using Tasks.Infrastructure.Common.Extensions;

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
        public async Task<AppTask?> GetAsync(string Id)
        {
            return await _applicationDbContext.AppTasks.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<AppTask>> GetAllProjectTasksAsync(string projectId)
        {
            return await _applicationDbContext.AppTasks.Where(x => x.ProjectId == projectId)
                                                       .ToListAsync();
        }
        public async Task<(List<AppTask> appTasks, int totalCount)> GetAsync(string userId, GetAppTasksQuery @params)
        {

            var query = _applicationDbContext.AppTasks.AsQueryable();

            if (string.IsNullOrWhiteSpace(@params.OrderBy) == false && @params.IsDescending.HasValue)
            {
                query.OrderBy(@params.OrderBy, @params.IsDescending.Value);
            }

            query = string.IsNullOrWhiteSpace(@params.Search) switch
            {
                true => query.Where(x => x.ProjectId == @params.ProjectId), //pobiera wszyskie zadania dla danego projektu
                false => query.Where(x => x.ProjectId == userId && x.Name.StartsWith(@params.Search)),
            };

            int totalCount = await query.CountAsync();

            var projects = await query.Skip(@params.Skip)
                                      .Take(@params.Take)
                                      .ToListAsync();

            return (projects, totalCount);
        }
        //public async Task<(List<Project> Projects, int TotalCount)> Get(string userId, GetProjectsQuery appParams)
        //{

        //    var query = ProjectMembersQuery.AsQueryable();

        //    query = query.Include(pm => pm.MotherProject);
        //    //.ThenInclude(p => p.ProjectMembers);

        //    if (string.IsNullOrWhiteSpace(appParams.OrderBy) == false && appParams.IsDescending.HasValue)
        //    {
        //        query.OrderBy(appParams.OrderBy, appParams.IsDescending.Value);
        //    }

        //    query = string.IsNullOrWhiteSpace(appParams.Search) switch
        //    {
        //        true => query.Where(x => x.UserId == userId),
        //        false => query.Where(x => x.UserId == userId && x.MotherProject.Name.StartsWith(appParams.Search)),
        //    };

        //    int totalCount = await query.CountAsync();

        //    var projects = await query.Select(pm => pm.MotherProject)
        //                              .Skip(appParams.Skip)
        //                              .Take(appParams.Take)
        //                              .ToListAsync();

        //    return (projects, totalCount);
        //}
    }
}
