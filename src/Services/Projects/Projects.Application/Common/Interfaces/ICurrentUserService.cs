using System.Security.Claims;

namespace Projects.Application.Common.Interfaces;

public interface ICurrentUserService
{
    public ClaimsPrincipal GetUser();
    public string GetUserId();
    public string GetUserEmail();
    public string? GetUserPhoto();
}
