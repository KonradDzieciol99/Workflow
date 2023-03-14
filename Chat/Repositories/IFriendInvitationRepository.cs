using Chat.Common.Models;
using Chat.Entity;

namespace Chat.Repositories
{
    public interface IFriendInvitationRepository : IRepository<FriendInvitation>
    {
        public Task<bool> checkIfExistsAsync(string InviterUserId, string InvitedUserId);
        public Task<IEnumerable<string>> findAllFriendIds(string UserId);
        public Task<IEnumerable<FriendInvitation>> GetAllInvitations(string UserId);
        public Task<FriendInvitation?> GetFriendInvitation(string InviterUserId, string InvitedUserId);
        public Task<IEnumerable<FriendInvitation>> GetAllFriends(string UserId);
        //public Task<List<FriendInvitation>> FindAllAsync(string userId, string[] searchedUsersIds);
        public Task<List<UserFriendStatusToTheUser>?> GetFriendsStatusAsync(string userId, string[] searchedUsersIds);
        Task<FriendInvitation?> GetInvitationAsync(string userId, string recipientId);
    }
}
