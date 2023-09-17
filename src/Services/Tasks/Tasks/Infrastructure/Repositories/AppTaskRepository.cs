using Microsoft.EntityFrameworkCore;
using Tasks.Application.AppTasks.Queries;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Common.Extensions;
using Tasks.Infrastructure.DataAccess;

namespace Tasks.Infrastructure.Repositories;

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
        return await _applicationDbContext.AppTasks.Include(t => t.TaskAssignee)
                                                   .Include(t => t.TaskLeader)
                                                   .SingleOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<List<AppTask>> GetAllProjectTasksAsync(string projectId)
    {
        return await _applicationDbContext.AppTasks.Include(t => t.TaskAssignee)
                                                   .Include(t => t.TaskLeader)
                                                   .Where(x => x.ProjectId == projectId)
                                                   .ToListAsync();
    }
    public async Task<(List<AppTask> appTasks, int totalCount)> GetAsync(string userId, GetAppTasksQuery @params)
    {

        var query = _applicationDbContext.AppTasks.Include(t => t.TaskLeader)
                                                  .Include(t => t.TaskAssignee)
                                                  .AsQueryable();

        if (string.IsNullOrWhiteSpace(@params.OrderBy) == false && @params.IsDescending.HasValue)
        {
            query.OrderBy(@params.OrderBy, @params.IsDescending.Value);
        }

        query = string.IsNullOrWhiteSpace(@params.Search) switch
        {
            true => query.Where(x => x.ProjectId == @params.ProjectId),
            false => query.Where(x => x.ProjectId == @params.ProjectId && x.Name.StartsWith(@params.Search)),
        };

        int totalCount = await query.CountAsync();

        var projects = await query.Skip(@params.Skip)
                                  .Take(@params.Take)
                                  .ToListAsync();

        return (projects, totalCount);
    }
}
