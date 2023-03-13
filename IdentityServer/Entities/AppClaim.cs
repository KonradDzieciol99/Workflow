using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities
{
    public class AppClaim : IdentityUserClaim<string>
    {
        public AppUser User { get; set; }
    }
}
