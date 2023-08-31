using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public class FriendRequestAcceptedEvent : IntegrationEvent
{
    public FriendRequestAcceptedEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string invitationAcceptingUserId, string invitationAcceptingUserEmail, string? invitationAcceptingUserPhotoUrl)
    {
        InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
        InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
        InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
        InvitationAcceptingUserId = invitationAcceptingUserId ?? throw new ArgumentNullException(nameof(invitationAcceptingUserId));
        InvitationAcceptingUserEmail = invitationAcceptingUserEmail ?? throw new ArgumentNullException(nameof(invitationAcceptingUserEmail));
        InvitationAcceptingUserPhotoUrl = invitationAcceptingUserPhotoUrl;
    }

    public string InvitationSendingUserId { get; set; }
    public string InvitationSendingUserEmail { get; set; }
    public string? InvitationSendingUserPhotoUrl { get; set; }

    public string InvitationAcceptingUserId { get; set; }
    public string InvitationAcceptingUserEmail { get; set; }
    public string? InvitationAcceptingUserPhotoUrl { get; set; }
}
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
