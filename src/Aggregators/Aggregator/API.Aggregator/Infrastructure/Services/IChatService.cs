using API.Aggregator.Application.Commons.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IChatService
{
    Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids);
}
