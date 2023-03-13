﻿using Chat.Common.Models;
using Chat.Entity;
using Chat.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Chat.Repositories
{
    public class FriendInvitationRepository : Repository<FriendInvitation>, IFriendInvitationRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FriendInvitationRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<bool> checkIfExistsAsync(string InviterUserId, string InvitedUserId)
        {

            var friendsInvitation = await _applicationDbContext.FriendsInvitation.FindAsync(InviterUserId, InvitedUserId);

            if (friendsInvitation is null)
            {
                return false;
            }

            return false;
        }
        public async Task<IEnumerable<string>> findAllFriendIds(string UserId)
        {
            var friendsInvitationIds = await _applicationDbContext.FriendsInvitation
                                            .Where(x => x.InviterUserId == UserId || x.InvitedUserId == UserId)
                                            .Select(x =>
                                                x.InviterUserId == UserId ? x.InvitedUserId : x.InvitedUserId == UserId ? x.InviterUserId : ""
                                            ).ToListAsync();

            var filteredIdsList = friendsInvitationIds.Where(fl => string.IsNullOrEmpty(fl) == false);

            return filteredIdsList;
            //IsAlreadyInvited = x.FriendInvitationSent.Select(b => b.InviterUser.Email).Contains(userEmail) ? true :
            //                   x.FriendInvitationSent.Select(b => b.InvitedUser.Email).Contains(userEmail) ? true : false,
            //if (friendsInvitation is null)
            //{
            //    return new FreindInvitationRelationStatus() { Confirmed = false, IsAlreadyInvited = false };
            //}
            //var freindInvitationRelationStatus = new FreindInvitationRelationStatus();

            //freindInvitationRelationStatus.IsAlreadyInvited = true;
            //freindInvitationRelationStatus.Confirmed = friendsInvitation.Confirmed;

            //return freindInvitationRelationStatus;
        }
        public async Task<IEnumerable<FriendInvitation>> GetAllFriends(string UserId)//confimed invitations
        {
            var Friends = await _applicationDbContext.FriendsInvitation
                                .Where(x => (x.InviterUserId == UserId || x.InvitedUserId == UserId) && x.Confirmed == true)
                                .ToListAsync();

            return Friends;

        }
        public async Task<IEnumerable<FriendInvitation>> GetAllInvitations(string UserId)
        {
            var Friends = await _applicationDbContext.FriendsInvitation
                                .Where(x => x.InvitedUserId == UserId && x.Confirmed == false)
                                .ToListAsync();

            return Friends;
        }
        public async Task<FriendInvitation?> GetFriendInvitation(string InviterUserId, string InvitedUserId)
        {
            var friendInvitations = await _applicationDbContext.FriendsInvitation.FindAsync(InviterUserId, InvitedUserId);
            return friendInvitations;//TODO DELETE this
        }

        //public async Task<List<FriendInvitation>?> FindAllAsync(string userId, string[] searchedUsersIds)
        //{
        //    var keys = searchedUsersIds.Select(searchedUserId => new { searchedUserId = searchedUserId, userId = userId} );
        //    return await _applicationDbContext.FriendsInvitation.Where(e => keys.Contains(new { searchedUserId = e.InviterUserId, userId = userId })).ToListAsync();

        //}
        public async Task<List<UserFriendStatusToTheUser>?> GetFriendsStatusAsync(string userId, string[] searchedUsersIds)
        {
            //var keys = searchedUsersIds.Select(searchedUserId => new { searchedUserId = searchedUserId, userId = userId });
            //return await _applicationDbContext.FriendsInvitation.Where(
            //        e => keys.Contains(new { searchedUserId = e.InviterUserId, userId = userId })
            //        //|| keys.Contains(new { searchedUserId = userId, userId = e.InviterUserId }) !!!!!!!!!!!!!
            //        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //    ).ToListAsync();

            //keys.Contains(new { searchedUserId = x.InviterUserId, userId = userId })
            //|| keys.Contains(new { searchedUserId = userId, userId = x.InviterUserId })

            var keys = searchedUsersIds.Select(searchedUserId => new testclass{ searchedUserId = searchedUserId, userId = userId });
            return await _applicationDbContext.FriendsInvitation
                .Where(x =>
                (searchedUsersIds.Contains(x.InvitedUserId) && x.InviterUserId == userId)
                || (searchedUsersIds.Contains(x.InviterUserId) && x.InvitedUserId == userId)
                //keys.Any(c=>c.searchedUserId == x.InviterUserId && c.userId== x.InvitedUserId)
                //keys.Any(c=>c.searchedUserId==x.InvitedUserId)
                //|| keys.Any(c => c.searchedUserId == x.InvitedUserId && c.userId == x.InviterUserId)
                //|| keys.Contains(new { searchedUserId = userId, userId = x.InviterUserId })
                )
                .Select(e =>
                    new UserFriendStatusToTheUser()
                    {
                        Status = e.Confirmed ? UserFriendStatusType.Friend
                        : (e.InviterUserId == userId ? UserFriendStatusType.InvitedByYou : UserFriendStatusType.InvitedYou),
                        UserId = e.InviterUserId != userId ? e.InviterUserId : e.InvitedUserId,
                    }
                 )
                .ToListAsync();

            //var keys = searchedUsersIds.Select(searchedUserId => (searchedUserId, userId));
            //return await _applicationDbContext.FriendsInvitation
            //    .Where(x => keys.Contains((x.InviterUserId, userId)) || keys.Contains((userId, x.InviterUserId)))
            //    .Select(e=>
            //        new UserFriendStatusToTheUser()
            //        {
            //            Status = e.Confirmed ? UserFriendStatusType.Friend 
            //            : (e.InviterUserId == userId ? UserFriendStatusType.InvitedByYou : UserFriendStatusType.InvitedYou),
            //            UserId = e.InviterUserId != userId ? e.InviterUserId : e.InvitedUserId,
            //        }
            //     )
            //    .ToListAsync();
        }
        public async Task<bool> CheckIfUsersAreFriends(string userId,string recipientId)
        {
            return await _applicationDbContext.FriendsInvitation
                    .AnyAsync(x => ((x.InviterUserId == userId && x.InvitedUserId == recipientId)
                    || (x.InviterUserId == recipientId && x.InvitedUserId == userId)) && x.Confirmed == true);
        }
        public async Task<FriendInvitation?> GetInvitationAsync(string userId, string recipientId)
        {
            return await _applicationDbContext.FriendsInvitation
                    .FirstOrDefaultAsync(x => (x.InviterUserId == userId && x.InvitedUserId == recipientId)
                    || (x.InviterUserId == recipientId && x.InvitedUserId == userId));
        }
    }

    public class testclass
    {
        public string searchedUserId { get; set; }
        public string userId { get; set; }
    }
}
