using System.Security.Claims;

namespace Tasks.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal User { get; }
        string UserEmail { get; }
        string UserId { get; }
        string? UserPhoto { get; }
    }
}