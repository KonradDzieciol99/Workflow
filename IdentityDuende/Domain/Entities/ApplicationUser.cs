using Microsoft.AspNetCore.Identity;

namespace IdentityDuende.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? PictureUrl { get; set; }
}
