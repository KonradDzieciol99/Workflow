using Chat.Dto;
using Chat.Entity;

namespace Chat.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserByIdAsync(string Id);
        //public Task<IEnumerable<User>> FindUsersByEmailAsync(string email, string emailOfSearcher);
        public Task<IEnumerable<SearchedUserDto>> FindUsersByEmailAsync(string seekerEmail, string emailOfSearchedUser);
        //public Task<IEnumerable<SearchedUserDto>> TEST(string userEmail, string searchedUseremail, IEnumerable<string> IdsInRelation);
        
    }
}
