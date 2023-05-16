using System.Security.Claims;

namespace Projects.Application.Common.ServiceInterfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    ClaimsPrincipal? User { get; }
}
