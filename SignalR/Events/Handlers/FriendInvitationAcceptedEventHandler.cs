using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace SignalR.Events.Handlers
{
    public class FriendInvitationAcceptedEventHandler : IRequestHandler<FriendInvitationAcceptedEvent>
    {

        private readonly IHubContext<MessagesHub> _messagesHubContext;
        private readonly IDatabase _redisDb;

        public FriendInvitationAcceptedEventHandler(IConnectionMultiplexer connectionMultiplexer,
            IHubContext<MessagesHub> messagesHubContext)
        {
            this._messagesHubContext = messagesHubContext;
            this._redisDb = connectionMultiplexer.GetDatabase();
        }
        public async Task Handle(FriendInvitationAcceptedEvent request, CancellationToken cancellationToken)
        {

            await _messagesHubContext.Clients.User(request.InvitationSendingUserId).SendAsync("FriendInvitationAccepted", request);
            await _messagesHubContext.Clients.User(request.InvitationAcceptingUserId).SendAsync("FriendInvitationAccepted", request);

            var isOnline = await _redisDb.KeyExistsAsync($"presence-{request.InvitationSendingUserEmail}");
            if (isOnline)
                await _messagesHubContext.Clients.User(request.InvitationAcceptingUserEmail).SendAsync("UserIsOnline", new { UserId = request.InvitationSendingUserId, UserEmail = request.InvitationSendingUserEmail, PhotoUrl= request.InvitationSendingUserPhotoUrl });
            
            isOnline = await _redisDb.KeyExistsAsync($"presence-{request.InvitationAcceptingUserEmail}");
            if (isOnline)
                await _messagesHubContext.Clients.User(request.InvitationSendingUserId).SendAsync("UserIsOnline", new { UserId = request.InvitationAcceptingUserId, UserEmail = request.InvitationAcceptingUserEmail, PhotoUrl = request.InvitationAcceptingUserPhotoUrl });

            return;
        }
    }
}
