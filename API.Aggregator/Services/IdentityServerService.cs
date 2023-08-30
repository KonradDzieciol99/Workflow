using API.Aggregator.Models;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Services;

public class IdentityServerService : BaseHttpService, IIdentityServerService
{
    private readonly string _identityUrl;

    public IdentityServerService(HttpClient client, IConfiguration configuration) : base(client)
    {
        _identityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException(_identityUrl);
    }

    public async Task<UserDto?> CheckIfUserExistsAsync(string email, string token)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/CheckIfUserExists/{email}");

        return await this.SendAsync<UserDto?>(new ApiRequest(HttpMethod.Get, sb.ToString(), null, token));
    }
    public async Task<List<UserDto>> SearchAsync(string email, string token)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/search/{email}");

        return await this.SendAsync<List<UserDto>>(new ApiRequest(HttpMethod.Get, sb.ToString(), null, token));
    }
}
