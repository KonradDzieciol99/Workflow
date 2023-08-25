using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Queries;
using Chat.Domain.Entity;
using Chat.Infrastructure.DataAccess;
using MessageBus.Models;
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
        //return await _dbContext.FriendRequests.FindAsync(sourceUserId, targetUserId);
        return await _dbContext.FriendRequests.SingleOrDefaultAsync(x => (x.InviterUserId == sourceUserId && x.InvitedUserId == targetUserId) ||
                                                                         (x.InviterUserId == targetUserId && x.InvitedUserId == sourceUserId) 
                                                                         );

                    //.FirstOrDefaultAsync(x => (x.InviterUserId == userId && x.InvitedUserId == recipientId)
                    //|| (x.InviterUserId == recipientId && x.InvitedUserId == userId));
    }

    //public async Task<List<FriendRequest>> GetAsync(string userId) //old
    //{
    //    return await _dbContext.FriendRequests.Where(fr => fr.InviterUserId == userId || fr.InvitedUserId == userId)
    //                                          .ToListAsync();
    //}

    public async Task<List<FriendRequest>> GetReceivedFriendRequestsAsync(string userId)
    {
        //return await _dbContext.FriendRequests.Where(x =>(x.InviterUserId == userId || x.InvitedUserId == userId) && x.Confirmed == false)
        //                                      .ToListAsync();
        return await _dbContext.FriendRequests.Where(x => x.InvitedUserId == userId && x.Confirmed == false)
                                              .ToListAsync();
    }
    public async Task<List<FriendRequest>> GetConfirmedAsync(string UserId,GetConfirmedFriendRequestsQuery @params)
    {


        var query = _dbContext.FriendRequests.Where(x => (x.InviterUserId == UserId || x.InvitedUserId == UserId) && x.Confirmed == true);

        if (!string.IsNullOrWhiteSpace(@params.Search))
            query = query.Where(x=> x.InviterUserId != UserId ? x.InviterUserEmail.StartsWith(@params.Search) : x.InvitedUserEmail.StartsWith(@params.Search));

        return await query.Skip(@params.Skip)
                          .Take(@params.Take)
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
    public async Task<List<FriendStatusDto>> CheckUsersToUserStatusAsync(string userId,  List<string> userIds)
    {
        var userStatuses = await _dbContext.FriendRequests
            .Where(r =>
                (r.InviterUserId == userId && userIds.Contains(r.InvitedUserId)) ||
                (r.InvitedUserId == userId && userIds.Contains(r.InviterUserId))
                //(r.InviterUserId != userId && userIds.Contains(r.InviterUserId) && r.InvitedUserId == userId)
            )
            .Select(r => new FriendStatusDto
            {
                UserId = r.InviterUserId == userId ? r.InvitedUserId : r.InviterUserId,
                Status = r.Confirmed
                    ? FriendStatusType.Friend
                    : (r.InviterUserId == userId
                        ? FriendStatusType.InvitedByYou
                        : FriendStatusType.InvitedYou)

            }).ToListAsync();

        return userIds.Select(
                 id => userStatuses.FirstOrDefault(s => s.UserId == id)
                 ?? new FriendStatusDto { UserId = id, Status = FriendStatusType.Stranger }
                 )
                 .ToList();
    }



    //public async Task<List<UserDto>> GetConfirmedUsersAsync(string UserId)
    //{
    //    return await _dbContext.FriendRequests
    //                        .Where(x => (x.InviterUserId == UserId || x.InvitedUserId == UserId) && x.Confirmed == true)
    //                        .ToListAsync();
    //}
}
