using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class ChatService : BaseHttpService, IChatService
{
    private readonly string _chatServiceUrl;
    public ChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        _chatServiceUrl = configuration.GetValue<string>("urls:internal:chat") ?? throw new ArgumentNullException(nameof(configuration)); ;
    }

    public async Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids)
    {
        var sb = new StringBuilder(_chatServiceUrl);
        sb.Append($"/api/FriendRequests/GetFriendsStatus?usersIds={string.Join("&usersIds=", Ids)}");

        return await SendAsync<List<FriendStatusDto>>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }
}
