using Microsoft.EntityFrameworkCore;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.DataAccess;

namespace Tasks.Infrastructure.Repositories;

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
    public async Task<int> ExecuteRemoveAsync(string projectMemberId)
    {
        return await applicationDbContext.ProjectMembers.Where(x => x.Id == projectMemberId)
                                                        .ExecuteDeleteAsync();
    }
    public async Task<int> ExecuteUpdateAsync(string projectMemberId, ProjectMemberType projectMemberType, InvitationStatus invitationStatus)
    {
        return await applicationDbContext.ProjectMembers.Where(x => x.Id == projectMemberId)
                                                            .ExecuteUpdateAsync(s =>
                                                            s.SetProperty(
                                                                n => n.Type,
                                                                projectMemberType)
                                                            .SetProperty(
                                                                b => b.invitationStatus,
                                                                invitationStatus)
                                                            );
    }
    public async Task<int> RemoveAllProjectMembersAsync(string projectId)
    {
        return await applicationDbContext.ProjectMembers.Where(x => x.ProjectId == projectId)
                                                        .ExecuteDeleteAsync();
    }
    public async Task<ProjectMember?> GetAsync(string Id)
    {
        return await applicationDbContext.ProjectMembers.SingleOrDefaultAsync(x => x.Id == Id);
    }
    public async Task<ProjectMember?> GetAsync(string userId, string projectId)
    {
        return await applicationDbContext.ProjectMembers.SingleOrDefaultAsync(x => x.UserId == userId && x.ProjectId == projectId);
    }
}
