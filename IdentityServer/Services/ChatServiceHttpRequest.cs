using Duende.IdentityServer.Models;
using IdentityServer.Common.Models;
using IdentityServer.Extensions;
using MediatR;
using System.Net.Http.Headers;

namespace IdentityServer.Services
{
    public class ChatServiceHttpRequest : IChatServiceHttpRequest
    {
        private readonly HttpClient _client;

        public ChatServiceHttpRequest(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public async Task<List<UserFriendStatusToTheUser>> GetFriendsStatus(string searcherId, IEnumerable<string> idOfSearchedUsers,string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.GetAsync($"/api/FriendInvitation/test2?searcherId={searcherId}&idOfSearchedUsers={string.Join(",", idOfSearchedUsers)}");
            return await response.ReadContentAs<List<UserFriendStatusToTheUser>>();
        }
    }
}
