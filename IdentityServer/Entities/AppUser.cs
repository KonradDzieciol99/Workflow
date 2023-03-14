using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<AppClaim> Claims { get; set; }
    }
}
