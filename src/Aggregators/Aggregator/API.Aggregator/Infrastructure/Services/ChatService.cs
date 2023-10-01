using API.Aggregator.Application.Common.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using HttpMessage.Services;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class ChatService : BaseHttpService, IChatService
{
    private readonly string _chatServiceUrl;

    public ChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));


        _chatServiceUrl =
            configuration.GetValue<string>("urls:internal:chat")
                ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:chat' is missing.");
    }

    public async Task<List<FriendStatusDto>> GetFriendsStatus(List<string> Ids, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_chatServiceUrl);
        sb.Append(
            $"/api/FriendRequests/GetFriendsStatus?usersIds={string.Join("&usersIds=", Ids)}"
        );

        return await SendAsync<List<FriendStatusDto>>(
            new ApiRequest(HttpMethod.Get, sb.ToString(), null), cancellationToken
        );
    }
}
