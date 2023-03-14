using IdentityServer.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Repositories
{
    public interface IIdentityUserRepository : IRepository<IdentityUser>
    {
        public Task<List<SearchedUser>> FindUsersByEmailAsync(string seekerEmail, string emailOfSearchedUser);
        public Task<UserDto?> GetUsersByEmailAsync(string email);

    }
}
