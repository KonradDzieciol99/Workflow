using Microsoft.AspNetCore.Identity;

namespace IdentityDuende.Entities;

public class ApplicationUser : IdentityUser
{
    public string? PictureUrl { get; set; }
}
