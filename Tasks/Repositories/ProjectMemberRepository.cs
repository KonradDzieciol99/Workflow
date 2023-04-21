using Microsoft.EntityFrameworkCore;
using Tasks.DataAccess;
using Tasks.Entity;

namespace Tasks.Repositories
{
    public class ProjectMemberRepository : Repository<ProjectMember>, IProjectMemberRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ProjectMemberRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public async Task<bool> CheckIfUserIsAMemberOfProject(string projectId, string userId)
        {
            return await applicationDbContext.ProjectMembers.AnyAsync(x => x.UserId == userId && x.ProjectId == projectId);
        }
        public async Task<int> RemoveAsync(string projectId, string userId)
        {
            return await applicationDbContext.ProjectMembers.Where(x => x.UserId == userId && x.ProjectId == projectId)
                                                            .ExecuteDeleteAsync();
        }
        public async Task<int> UpdateAsync(string projectId, string userId , ProjectMemberType projectMemberType)
        {
            return await applicationDbContext.ProjectMembers.Where(x => x.UserId == userId && x.ProjectId == projectId)
                                                                .ExecuteUpdateAsync(s => s.SetProperty(
                                                                    n => n.Type,
                                                                    n => projectMemberType));
        }
    }
}
