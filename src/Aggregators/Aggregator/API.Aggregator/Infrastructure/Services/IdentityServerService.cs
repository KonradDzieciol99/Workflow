using API.Aggregator.Application.Common.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using HttpMessage.Services;
using System.Text;
using System.Threading;

namespace API.Aggregator.Infrastructure.Services;

public class IdentityServerService : BaseHttpService, IIdentityServerService
{
    private readonly string _identityUrl;

    public IdentityServerService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        _identityUrl =
            configuration.GetValue<string>("urls:internal:identity")
                ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:identity' is missing.");
    }

    public async Task<UserDto?> CheckIfUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/CheckIfUserExists/{email}");

        return await SendAsync<UserDto?>(new ApiRequest(HttpMethod.Get, sb.ToString(), null), cancellationToken);
    }

    public async Task<List<UserDto>> SearchAsync(string email, int take, int skip, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/search/{email}?take={take}&skip={skip}");

        return await SendAsync<List<UserDto>>(new ApiRequest(HttpMethod.Get, sb.ToString(), null), cancellationToken);
    }
}
