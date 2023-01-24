
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}