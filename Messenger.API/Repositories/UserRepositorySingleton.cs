using Microsoft.EntityFrameworkCore;
using Socjal.API.Models;
using Socjal.API.Persistence;

namespace Socjal.API.Repositories
{
    public class UserRepositorySingleton : IUserRepositorySingleton
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public UserRepositorySingleton(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddUser(User orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.Users.Add(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
