using API.Aggregator.Models;

namespace API.Aggregator.Services;

public interface IIdentityServerService
{
    Task<UserDto?> CheckIfUserExistsAsync(string email, string token);
    Task<List<UserDto>> SearchAsync(string email, string token);
}