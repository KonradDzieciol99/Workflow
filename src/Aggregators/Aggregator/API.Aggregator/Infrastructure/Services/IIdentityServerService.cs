using API.Aggregator.Application.Common.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IIdentityServerService
{
    Task<UserDto?> CheckIfUserExistsAsync(string email, CancellationToken cancellationToken);
    Task<List<UserDto>> SearchAsync(string email, int take, int skip, CancellationToken cancellationToken);
}
