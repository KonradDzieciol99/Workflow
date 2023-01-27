using Socjal.API.Models;
using Socjal.API.Persistence;

namespace Socjal.API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
