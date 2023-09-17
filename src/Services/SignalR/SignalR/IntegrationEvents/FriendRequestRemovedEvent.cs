using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public record FriendRequestRemovedEvent(string ActionInitiatorUserId, string ActionInitiatorUserEmail, string? ActionInitiatorUserPhotoUrl, string FriendToRemoveUserId, string FriendToRemoveUserEmail, string? FriendToRemoveUserPhotoUrl) : IntegrationEvent;
public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;

    public FriendRequestRemovedEventHandler(IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext ?? throw new ArgumentNullException(nameof(messagesHubContext));
    }
    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        await _messagesHubContext.Clients.Users(request.FriendToRemoveUserId).SendAsync("FriendRequestRemoved", request.ActionInitiatorUserEmail, cancellationToken: cancellationToken);
        return;
    }
}
