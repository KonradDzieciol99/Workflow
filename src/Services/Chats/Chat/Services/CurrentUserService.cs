using Chat.Application.Common.Exceptions;
using System.Security.Claims;

namespace Chat.Services;

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
        ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));

    public string GetUserEmail() =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
        ?? throw new ArgumentNullException(nameof(ClaimTypes.Email));

    public string? GetUserPhoto() =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("picture");
}
