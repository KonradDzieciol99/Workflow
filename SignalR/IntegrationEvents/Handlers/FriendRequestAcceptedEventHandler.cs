using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents.Handlers;

public class FriendRequestAcceptedEventHandler : IRequestHandler<FriendRequestAcceptedEvent>
{

    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly IDatabase _redisDb;

    public FriendRequestAcceptedEventHandler(IConnectionMultiplexer connectionMultiplexer,
        IHubContext<MessagesHub> messagesHubContext, IHubContext<PresenceHub> presenceHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _presenceHubContext = presenceHubContext;
        _redisDb = connectionMultiplexer.GetDatabase();
    }
    public async Task Handle(FriendRequestAcceptedEvent request, CancellationToken cancellationToken)
    {

        var friendInvitationDto = new FriendInvitationDto(request.InvitationSendingUserId, request.InvitationSendingUserEmail, request.InvitationSendingUserPhotoUrl, request.InvitationAcceptingUserId, request.InvitationAcceptingUserEmail, request.InvitationAcceptingUserPhotoUrl, true);

        await _messagesHubContext.Clients.User(request.InvitationSendingUserId).SendAsync("FriendInvitationAccepted", friendInvitationDto);

        await _messagesHubContext.Clients.User(request.InvitationAcceptingUserId).SendAsync("FriendInvitationAccepted", friendInvitationDto);

        var isOnline = await _redisDb.KeyExistsAsync($"presence-{request.InvitationSendingUserEmail}");
        if (isOnline)
            // await _messagesHubContext.Clients.User(request.InvitationAcceptingUserId).SendAsync("UserIsOnline", new UserDto(request.InvitationSendingUserId, request.InvitationSendingUserEmail, request.InvitationSendingUserPhotoUrl) );
            await _presenceHubContext.Clients.User(request.InvitationAcceptingUserId).SendAsync("UserIsOnline", request.InvitationSendingUserEmail);

        isOnline = await _redisDb.KeyExistsAsync($"presence-{request.InvitationAcceptingUserEmail}");
        if (isOnline)
            //await _messagesHubContext.Clients.User(request.InvitationSendingUserId).SendAsync("UserIsOnline", new UserDto(request.InvitationAcceptingUserId, request.InvitationAcceptingUserEmail, request.InvitationAcceptingUserPhotoUrl));
            await _presenceHubContext.Clients.User(request.InvitationSendingUserId).SendAsync("UserIsOnline", request.InvitationAcceptingUserEmail);

        return;
    }
}
