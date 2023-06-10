using System.Security.Claims;

namespace Notification.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal User { get; }
        string UserEmail { get; }
        string UserId { get; }
        string? UserPhoto { get; }
    }
}