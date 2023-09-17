using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public record FriendRequestAddedEvent(string InvitationSendingUserId, string InvitationSendingUserEmail, string? InvitationSendingUserPhotoUrl, string InvitedUserId, string InvitedUserEmail, string? InvitedUserPhotoUrl) : IntegrationEvent;
public class FriendRequestAddedEventHandler : IRequestHandler<FriendRequestAddedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;

    public FriendRequestAddedEventHandler(IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext ?? throw new ArgumentNullException(nameof(messagesHubContext));
    }
    public async Task Handle(FriendRequestAddedEvent request, CancellationToken cancellationToken)
    {
        await _messagesHubContext.Clients.User(request.InvitedUserId).SendAsync(
            "NewInvitationToFriendsReceived",
            new FriendInvitationDto(request.InvitationSendingUserId,
                                    request.InvitationSendingUserEmail,
                                    request.InvitationSendingUserPhotoUrl,
                                    request.InvitedUserId,
                                    request.InvitedUserEmail,
                                    request.InvitedUserPhotoUrl,
                                    false), cancellationToken: cancellationToken);
    }
}
