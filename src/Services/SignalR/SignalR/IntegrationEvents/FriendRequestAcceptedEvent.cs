using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public record FriendRequestAcceptedEvent(
    string InvitationSendingUserId,
    string InvitationSendingUserEmail,
    string? InvitationSendingUserPhotoUrl,
    string InvitationAcceptingUserId,
    string InvitationAcceptingUserEmail,
    string? InvitationAcceptingUserPhotoUrl
) : IntegrationEvent;

public class FriendRequestAcceptedEventHandler : IRequestHandler<FriendRequestAcceptedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly IDatabase _redisDb;

    public FriendRequestAcceptedEventHandler(
        IConnectionMultiplexer connectionMultiplexer,
        IHubContext<MessagesHub> messagesHubContext,
        IHubContext<PresenceHub> presenceHubContext
    )
    {
        _messagesHubContext =
            messagesHubContext ?? throw new ArgumentNullException(nameof(messagesHubContext));
        _presenceHubContext =
            presenceHubContext ?? throw new ArgumentNullException(nameof(presenceHubContext));
        _redisDb =
            connectionMultiplexer.GetDatabase()
            ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task Handle(
        FriendRequestAcceptedEvent request,
        CancellationToken cancellationToken
    )
    {
        var friendInvitationDto = new FriendInvitationDto(
            request.InvitationSendingUserId,
            request.InvitationSendingUserEmail,
            request.InvitationSendingUserPhotoUrl,
            request.InvitationAcceptingUserId,
            request.InvitationAcceptingUserEmail,
            request.InvitationAcceptingUserPhotoUrl,
            true
        );

        await _messagesHubContext.Clients
            .User(request.InvitationSendingUserId)
            .SendAsync(
                "FriendInvitationAccepted",
                friendInvitationDto,
                cancellationToken: cancellationToken
            );

        await _messagesHubContext.Clients
            .User(request.InvitationAcceptingUserId)
            .SendAsync(
                "FriendInvitationAccepted",
                friendInvitationDto,
                cancellationToken: cancellationToken
            );

        var isOnline = await _redisDb.KeyExistsAsync(
            $"presence-{request.InvitationSendingUserEmail}"
        );

        if (isOnline)
            await _presenceHubContext.Clients
                .User(request.InvitationAcceptingUserId)
                .SendAsync(
                    "UserIsOnline",
                    request.InvitationSendingUserEmail,
                    cancellationToken: cancellationToken
                );

        isOnline = await _redisDb.KeyExistsAsync(
            $"presence-{request.InvitationAcceptingUserEmail}"
        );
        if (isOnline)
            await _presenceHubContext.Clients
                .User(request.InvitationSendingUserId)
                .SendAsync(
                    "UserIsOnline",
                    request.InvitationAcceptingUserEmail,
                    cancellationToken: cancellationToken
                );

        return;
    }
}
