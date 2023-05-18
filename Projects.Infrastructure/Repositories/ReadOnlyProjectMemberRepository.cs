using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Projects.Infrastructure.DataAccess;
using Projects.Infrastructure.Common;
using Projects.Domain.Common.Models;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;

namespace Projects.Infrastructure.Repositories
{
    public class ReadOnlyProjectMemberRepository : IReadOnlyProjectMemberRepository
    {
        private readonly IQueryable<ProjectMember> ProjectMembersQuery;

        public ReadOnlyProjectMemberRepository(ApplicationDbContext applicationDbContext)
        {
            this.ProjectMembersQuery = applicationDbContext.ProjectMembers.AsNoTracking();
        }
        public async Task<Project?> GetOneAsync(string projectName, string userId)
        {
            return await ProjectMembersQuery.Include(pm => pm.MotherProject)
                                                            .ThenInclude(p => p.ProjectMembers)
                                                            .Where(x => x.UserId == userId && x.MotherProject.Name == projectName)
                                                            .Select(x => x.MotherProject)
                                                            .FirstOrDefaultAsync();
        }
        public async Task<ProjectMember?> GetAsync(string projectMemberId)
        {
            return await ProjectMembersQuery.SingleOrDefaultAsync(x => x.Id == projectMemberId);
        }
        public async Task<ProjectMember?> GetProjectMemberAsync(string projectId, string userId)
        {
            return await ProjectMembersQuery.SingleOrDefaultAsync(x => x.UserId == userId && x.MotherProject.Id == projectId);
        }

        /// <summary>
        /// Excluding members.
        /// </summary>
        public async Task<(List<Project> Projects, int TotalCount)> Get(string userId, AppParams appParams)
        {

            var query = ProjectMembersQuery.AsQueryable();

            query = query.Include(pm => pm.MotherProject);
                         //.ThenInclude(p => p.ProjectMembers);

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
        public async Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId)
        {
            return await ProjectMembersQuery.AnyAsync(x => x.UserId == userId && x.ProjectId == projectId);
        }
        public async Task<bool> CheckIfUserHasRightsToMenageUserAsync(string projectId, string userId)
        {
            return await ProjectMembersQuery.AnyAsync(x => x.UserId == userId && x.ProjectId == projectId
                                                                    && (x.Type == ProjectMemberType.Admin || x.Type == ProjectMemberType.Leader));
        }
        public async Task<bool> CheckIfUserIsALeaderAsync(string projectId, string userId)
        {
            return await ProjectMembersQuery.AnyAsync(x => x.UserId == userId && x.ProjectId == projectId
                                                                    && x.Type == ProjectMemberType.Leader);
        }
        public async Task<int> RemoveAsync(string projectId, string userId)
        {
            return await ProjectMembersQuery.Where(x => x.UserId == userId && x.MotherProject.Id == projectId
                                                                    && x.Type != ProjectMemberType.Leader).ExecuteDeleteAsync();
        }
    }
}
