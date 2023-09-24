using API.Aggregator.Application.Commons.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IIdentityServerService
{
    Task<UserDto?> CheckIfUserExistsAsync(string email);
    Task<List<UserDto>> SearchAsync(string email, int take, int skip);
}
