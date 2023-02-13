using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Socjal.API.Dto;
using Socjal.API.Entity;
using Socjal.API.Repositories;
using StackExchange.Redis;
using System.Security.Claims;

namespace Socjal.API
{
    public class PresenceHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDatabase _redisDb;
        public PresenceHub(IConnectionMultiplexer connectionMultiplexer, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _connectionMultiplexer = connectionMultiplexer;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            await _redisDb.SetAddAsync($"presence-{SenderEmail}", Context.ConnectionId);

            var friendsIds = await GetFriendsIds();
            await Clients.Users(friendsIds).SendAsync("UserIsOnline", SenderEmail);

            var currentUsers = await GetOnlineUsersEmails();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            await base.OnDisconnectedAsync(exception);
        }
        private async Task<IEnumerable<string>?> GetOnlineUsersEmails()
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");
            var SenderEmail = Context.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var friends = await this._unitOfWork.FriendInvitationRepository.GetAllFriends(userId);
            var friendsEmails = friends.Select(x => x.InviterUserEmail == SenderEmail ? x.InvitedUserEmail : x.InviterUserEmail);

            RedisKey[] redisKeys = friendsEmails.Select(x =>$"presence-{x}").Cast<RedisKey>().ToArray();

            //RedisKey[] redisKeys = friendsEmails.Cast<RedisKey>().ToArray();
            //var existingTable = await _redisDb.KeyExistsAsync(redisKeys);

            //await _redisDb.exist

            //RedisKey[] keys = { "key1", "key2", "key3" };

            //bool[] exists = _redisDb.KeyExists(keys);
            var existArray = await this.KeyExists(redisKeys);

            Dictionary<string, bool[]> dict = friendsEmails.Select((x, i) => new { key = x, value = existArray[i] }).GroupBy(x => x.key, y => y.value).ToDictionary(x => x.Key, y => y.ToArray());


            return friendsEmails;
        }
        private async Task<bool[]> KeyExists(RedisKey[] keys)
        {
            var tasks = keys.Select(key => _redisDb.KeyExistsAsync(key));
            var results = await Task.WhenAll(tasks);

            return results;
        }

        private async Task<IEnumerable<string>?> GetFriendsIds()
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");
            var SenderEmail = Context.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var friends = await this._unitOfWork.FriendInvitationRepository.GetAllFriends(userId);
            var friendIds = friends.Select(x => x.InviterUserEmail==SenderEmail ? x.InvitedUserId : x.InviterUserId);

            return friendIds;
        }

        public async Task SentFriendInvitationNotification(string Id)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");


        }
    }
}
