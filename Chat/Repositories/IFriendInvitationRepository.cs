﻿using Chat.Entity;

namespace Chat.Repositories
{
    public interface IFriendInvitationRepository : IRepository<FriendInvitation>
    {
        public Task<bool> checkIfExistsAsync(string InviterUserId, string InvitedUserId);
        public Task<IEnumerable<string>> findAllFriendIds(string UserId);
        public Task<IEnumerable<FriendInvitation>> GetAllInvitations(string UserId);
        public Task<FriendInvitation?> GetFriendInvitation(string InviterUserId, string InvitedUserId);
        public Task<IEnumerable<FriendInvitation>> GetAllFriends(string UserId);
    }
}
