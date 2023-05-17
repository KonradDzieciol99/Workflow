using System.Security.Claims;
using Projects.Application.Common.Exceptions;
using Projects.Application.Common.Interfaces;

namespace Projects.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException();
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedException();
}
