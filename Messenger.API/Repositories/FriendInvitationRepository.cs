﻿using Microsoft.EntityFrameworkCore;
using Socjal.API.Common.Models;
using Socjal.API.Entity;
using Socjal.API.Persistence;

namespace Socjal.API.Repositories
{
    public class FriendInvitationRepository : Repository<FriendInvitation>, IFriendInvitationRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FriendInvitationRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public async Task<bool> checkIfExistsAsync(string InviterUserId,string InvitedUserId)
        {

            var friendsInvitation = await _applicationDbContext.FriendsInvitation.FindAsync(InviterUserId, InvitedUserId );

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

            var filteredIdsList = friendsInvitationIds.Where(fl => string.IsNullOrEmpty(fl)==false);

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
        public async Task<IEnumerable<User>> GetAllFriends(string UserId)
        {
            var Friends = await _applicationDbContext.FriendsInvitation
                                .Where(x => (x.InviterUserId == UserId || x.InvitedUserId == UserId) && x.Confirmed == true )
                                .Select(x =>
                                   // x.InviterUserId == UserId ? x.InvitedUser : x.InvitedUserId == UserId ? x.InviterUser : new User() { }
                                    x.InviterUserId == UserId ? x.InvitedUser :  x.InviterUser 
                                ).ToListAsync();

            return Friends;
            //////////////zle!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        public async Task<IEnumerable<FriendInvitation>> GetAllInvitations(string UserId)
        {
            var Friends = await _applicationDbContext.FriendsInvitation
                                .Where(x => x.InvitedUserId == UserId && x.Confirmed == false )
                                .ToListAsync();

            return Friends;
        }
        public async Task<FriendInvitation?> GetFriendInvitation(string InviterUserId, string InvitedUserId)
        {
            var friendInvitations= await _applicationDbContext.FriendsInvitation.FindAsync(InviterUserId, InvitedUserId);
            return friendInvitations;
        }
    }
}
