using Projects.Application.Common.Exceptions;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public ClaimsPrincipal GetUser() =>
        _httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedException();

    public string GetUserId() =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException($"Claim '{nameof(ClaimTypes.NameIdentifier)}' not found.");

    public string GetUserEmail() =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException($"Claim '{nameof(ClaimTypes.Email)}' not found.");

    public string? GetUserPhoto() =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("picture");
}
