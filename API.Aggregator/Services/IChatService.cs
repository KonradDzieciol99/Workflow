using API.Aggregator.Models;

namespace API.Aggregator.Services;

public interface IChatService
{
    Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids, string token);
}