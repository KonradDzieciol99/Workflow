using System.Security.Claims;
using Tasks.Application.Common.Exceptions;

namespace Tasks.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
    }
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedException();
    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
    public string UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? throw new ArgumentNullException(nameof(ClaimTypes.Email));
    public string? UserPhoto => _httpContextAccessor.HttpContext?.User?.FindFirstValue("picture");
}
