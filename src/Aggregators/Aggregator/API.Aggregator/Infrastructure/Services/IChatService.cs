using API.Aggregator.Application.Common.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IChatService
{
    Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids, CancellationToken cancellationToken);
}
