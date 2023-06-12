using Chat.Domain.Entity;
using Chat.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chat.Infrastructure.Repositories;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FriendRequestRepository(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }
    public void Add(FriendRequest entity)
    {
        _dbContext.FriendRequests.Add(entity);
    }
    public void AddRange(IEnumerable<FriendRequest> entities)
    {
        _dbContext.FriendRequests.AddRange(entities);
    }
    public void Remove(FriendRequest entity)
    {
        _dbContext.FriendRequests.Remove(entity);
    }
    public async Task<FriendRequest?> GetAsync(string sourceUserId, string targetUserId)
    {
        return await _dbContext.FriendRequests.FindAsync(sourceUserId, targetUserId);

                    //.FirstOrDefaultAsync(x => (x.InviterUserId == userId && x.InvitedUserId == recipientId)
                    //|| (x.InviterUserId == recipientId && x.InvitedUserId == userId));
    }
    public async Task<List<FriendRequest>> GetAsync(string userId)
    {
        return await _dbContext.FriendRequests.Where(fr => fr.InviterUserId == userId || fr.InvitedUserId == userId)
                                              .ToListAsync();
    }
    public async Task<List<FriendRequest>> GetConfirmedAsync(string UserId)
    {
        return await _dbContext.FriendRequests
                            .Where(x => (x.InviterUserId == UserId || x.InvitedUserId == UserId) && x.Confirmed == true)
                            .ToListAsync();
    }
    public async Task<bool> CheckIfTheyShareFriendRequest(string userId,string targetUserId)
    {
        return await _dbContext.FriendRequests
                            .AnyAsync(x => (x.InviterUserId == userId && x.InvitedUserId == targetUserId) 
                            || (x.InviterUserId == targetUserId && x.InvitedUserId == userId));
    }
}
