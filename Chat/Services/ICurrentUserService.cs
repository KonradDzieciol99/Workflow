using System.Security.Claims;

namespace Chat.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal User { get; }
        string UserEmail { get; }
        string UserId { get; }
        string? UserPhoto { get; }
    }
}