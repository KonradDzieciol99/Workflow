using Socjal.API.Entity;

namespace Socjal.API.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<IEnumerable<User?>> FindUsersByEmailAsync(string email);
    }
}
