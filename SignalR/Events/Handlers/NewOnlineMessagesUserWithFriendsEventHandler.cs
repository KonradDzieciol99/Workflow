using MediatR;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace SignalR.Events.Handlers
{
    public class NewOnlineMessagesUserWithFriendsEventHandler : IRequestHandler<NewOnlineMessagesUserWithFriendsEvent>
    {
        private readonly IHubContext<MessagesHub> _messagesHubContext;
        private readonly IDatabase _redisDb;

        public NewOnlineMessagesUserWithFriendsEventHandler(IConnectionMultiplexer connectionMultiplexer,
            IHubContext<MessagesHub> messagesHubContext)
        {
            this._messagesHubContext = messagesHubContext;
            this._redisDb = connectionMultiplexer.GetDatabase();
        }
        public async Task Handle(NewOnlineMessagesUserWithFriendsEvent request, CancellationToken cancellationToken)
        {
            List<Task<bool>> listOfOnlineUsers = new();
            foreach (var item in request.NewOnlineUserChatFriends)
            {
                listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserEmail}"));// było UserId
            }
            var resoult = await Task.WhenAll(listOfOnlineUsers);

            List<SimpleUser> onlineUsers = new();
            for (int i = 0; i < request.NewOnlineUserChatFriends.Count(); i++)
            {
                if (resoult[i])
                {
                    onlineUsers.Add(request.NewOnlineUserChatFriends.ElementAt(i));
                }
            }
            await _messagesHubContext.Clients.User(request.NewOnlineUser.UserId).SendAsync("GetOnlineUsers", onlineUsers);
            return;
        }
    }
}
