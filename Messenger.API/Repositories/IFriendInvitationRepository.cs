using Socjal.API.Common.Models;
using Socjal.API.Entity;

namespace Socjal.API.Repositories
{
    public interface IFriendInvitationRepository : IRepository<FriendInvitation>
    {
        public Task<FreindInvitationRelationStatus> checkIfExistsAsync(string InviterUserId, string InvitedUserId);
        public  Task<IEnumerable<string>> findAllFriendIds(string UserId);

    }
}
