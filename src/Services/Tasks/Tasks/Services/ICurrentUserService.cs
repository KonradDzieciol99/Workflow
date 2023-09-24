using System.Security.Claims;

namespace Tasks.Services;

public interface ICurrentUserService
{
    public ClaimsPrincipal GetUser();
    public string GetUserId();
    public string GetUserEmail();
    public string? GetUserPhoto();
}
