using MediatR;
using MessageBus;
using MessageBus.Events;
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

            await _messagesHubContext.Clients.User(request.UserWhoseInvitationAccepted.UserId).SendAsync("FriendInvitationAccepted", request.FriendInvitationDto);

            var isOnline = await _redisDb.KeyExistsAsync($"presence-{request.UserWhoAcceptedInvitation.UserEmail}");
            if (isOnline)
            {
                await _messagesHubContext.Clients.User(request.UserWhoseInvitationAccepted.UserId).SendAsync("UserIsOnline", request.UserWhoAcceptedInvitation);
            }
            return;
        }
    }
}
