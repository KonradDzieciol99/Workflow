using IdentityServer.Common.Models;

namespace IdentityServer.Services
{
    public interface IChatServiceHttpRequest
    {
        Task<List<UserFriendStatusToTheUser>> GetFriendsStatus(string searcherId, IEnumerable<string> idOfSearchedUsers, string accessToken);
    }
}