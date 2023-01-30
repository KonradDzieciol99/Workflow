using Microsoft.EntityFrameworkCore;
using Socjal.API.Entity;
using Socjal.API.Persistence;

namespace Socjal.API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await this.applicationDbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
        public async Task<IEnumerable<User>> FindUsersByEmailAsync(string email)
        {
            return await this.applicationDbContext.Users.Where(user => user.Email.StartsWith(email)).ToListAsync();
        }
    }
}
