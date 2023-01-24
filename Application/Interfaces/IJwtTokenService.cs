using Domain.Identity.Entities;


namespace Core.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
