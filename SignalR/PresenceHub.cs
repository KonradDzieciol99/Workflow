using AutoMapper;
using Azure.Core;
using Azure.Core.Extensions;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace SignalR
{
    public class PresenceHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IAzureServiceBusSender _messageBus;

        //private readonly IMapper _mapper;
        private readonly IDatabase _redisDb;
        public PresenceHub(IConnectionMultiplexer connectionMultiplexer,IAzureServiceBusSender messageBus)
        {
            _connectionMultiplexer = connectionMultiplexer;
            this._messageBus = messageBus;
            //_mapper = mapper;
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var email = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");
            var id = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");

            await _redisDb.SetAddAsync($"presence-{email}", Context.ConnectionId);

            //var onlineUsers = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new User() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new User() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            var newOnlineUserEvent = new NewOnlineUserEvent() { NewOnlineUser = new SimpleUser() { UserEmail = email, UserId = id } };
            await _messageBus.PublishMessage(newOnlineUserEvent, "new-online-user-queue");


            //var notificationsSerialized = _redisDb.HashScan($"user-notification-{email}", pageSize: 5);
            //var notifications = notificationsSerialized.Select(notification => JsonSerializer.Deserialize<BaseMessage>(notification.Value.ToString())).ToList();
            //await Clients.Caller.SendAsync("NotificationThread", notifications);

            //var friendsIds = await GetFriendsIds();
            //await Clients.NewOnlineUserChatFriends(friendsIds).SendAsync("UserIsOnline", SenderEmail);

            //var currentUsers = await GetOnlineUsersEmails();
            //await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var email = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");
            var id = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");


            await _redisDb.SetRemoveAsync($"presence-{email}", Context.ConnectionId);
            var ConnectionLength = await _redisDb.SetLengthAsync($"presence-{email}");

            if (ConnectionLength == 0)
            {
                await _redisDb.KeyDeleteAsync($"presence-{email}");

                var newOnlineUserEvent = new NewOfflineUserEvent() { User = new SimpleUser() { UserEmail = email, UserId = id } };
                await _messageBus.PublishMessage(newOnlineUserEvent, "new-offline-user-topic");

                //string[] InvitedUsersConnectionIds = await GetInvitedUsers();
                //await Clients.Clients(InvitedUsersConnectionIds).SendAsync("UserIsOffline", userName);
                //event!
                //event zwrotny
            }

            await base.OnDisconnectedAsync(exception);
        }
        private async Task<IEnumerable<string>?> GetOnlineUsersEmails()
        {
            //string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");
            //var SenderEmail = Context.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            //var friends = await _unitOfWork.FriendInvitationRepository.GetAllFriends(userId);
            //var friendsEmails = friends.Select(x => x.InviterUserEmail == SenderEmail ? x.InvitedUserEmail : x.InviterUserEmail);

            //RedisKey[] redisKeys = friendsEmails.Select(x => $"presence-{x}").Cast<RedisKey>().ToArray();

            //RedisKey[] redisKeys = friendsEmails.Cast<RedisKey>().ToArray();
            //var existingTable = await _redisDb.KeyExistsAsync(redisKeys);

            //await _redisDb.exist

            //RedisKey[] keys = { "key1", "key2", "key3" };

            //bool[] exists = _redisDb.KeyExists(keys);
            //var existArray = await KeyExists(redisKeys);

            //Dictionary<string, bool[]> dict = friendsEmails.Select((x, i) => new { key = x, value = existArray[i] }).GroupBy(x => x.key, y => y.value).ToDictionary(x => x.Key, y => y.ToArray());


            //return friendsEmails;
            throw new HubException("asdasd");
        }
        private async Task<bool[]> KeyExists(RedisKey[] keys)
        {
            var tasks = keys.Select(key => _redisDb.KeyExistsAsync(key));
            var results = await Task.WhenAll(tasks);

            return results;
        }

        private async Task<IEnumerable<string>?> GetFriendsIds()
        {
            //string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("User cannot be identified");
            //var SenderEmail = Context.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            //var friends = await _unitOfWork.FriendInvitationRepository.GetAllFriends(userId);
            //var friendIds = friends.Select(x => x.InviterUserEmail == SenderEmail ? x.InvitedUserId : x.InviterUserId);

            //return friendIds;
            throw new HubException("asdasd");
        }

        public async Task SentFriendInvitationNotification(string Id)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");


        }
    }
}
