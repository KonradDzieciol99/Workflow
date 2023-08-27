using API.Aggregator.Models;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Services;

public class ChatService : BaseHttpService, IChatService
{
    private readonly string _chatServiceUrl;
    public ChatService(HttpClient client, IConfiguration configuration) : base(client)
    {
        this._chatServiceUrl = configuration.GetValue<string>("urls:internal:chat") ?? throw new ArgumentNullException(nameof(_chatServiceUrl)); ;
    }

    public async Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids, string token)
    {
        var sb = new StringBuilder(_chatServiceUrl);
        sb.Append($"/api/FriendRequests/GetFriendsStatus?usersIds={string.Join(",", Ids)}");

        return await this.SendAsync<List<FriendStatusDto>>(new ApiRequest()
        {
            HttpMethod = HttpMethod.Get,
            Url = sb.ToString(),
            AccessToken = token
        });
    }
}
