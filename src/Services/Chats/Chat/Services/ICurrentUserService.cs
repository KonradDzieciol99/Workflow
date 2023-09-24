using System.Security.Claims;

namespace Chat.Services;

public interface ICurrentUserService
{
    public ClaimsPrincipal GetUser();
    public string GetUserId();
    public string GetUserEmail();
    public string? GetUserPhoto();
}
