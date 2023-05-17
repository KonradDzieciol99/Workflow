using System.Security.Claims;

namespace Projects.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    ClaimsPrincipal User { get; }
}
