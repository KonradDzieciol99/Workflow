using System.Security.Claims;

namespace Projects.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserEmail { get; }
    string? UserPhoto { get; }
    ClaimsPrincipal User { get; }
}
