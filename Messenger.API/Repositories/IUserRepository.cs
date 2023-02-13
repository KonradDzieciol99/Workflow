using Socjal.API.Dto;
using Socjal.API.Entity;

namespace Socjal.API.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserByIdAsync(string Id);
        public Task<IEnumerable<User?>> FindUsersByEmailAsync(string email);
        public Task<IEnumerable<UserSearchedFriendInvitationDto>> TEST(string userEmail, string searchedUseremail, IEnumerable<string> IdsInRelation);

    }
}
