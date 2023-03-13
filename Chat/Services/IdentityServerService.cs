using HttpMessage;

namespace Chat.Services
{
    public class IdentityServerService : BaseHttpService, IIdentityServerService
    {
        public IdentityServerService(HttpClient client) : base(client)
        {
        }
        public async Task<T> CheckIfUserExistsAsync<T>(string email, string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = $"https://localhost:7122/api/IdentityUser/CheckIfUserExists/{email}",
                AccessToken = token
            });
        }
    }
}
