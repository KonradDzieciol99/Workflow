using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public class FriendRequestRemovedEvent : IntegrationEvent
{
    public FriendRequestRemovedEvent(string actionInitiatorUserId, string actionInitiatorUserEmail, string? actionInitiatorUserPhotoUrl, string friendToRemoveUserId, string friendToRemoveUserEmail, string? friendToRemoveUserPhotoUrl)
    {
        ActionInitiatorUserId = actionInitiatorUserId ?? throw new ArgumentNullException(nameof(actionInitiatorUserId));
        ActionInitiatorUserEmail = actionInitiatorUserEmail ?? throw new ArgumentNullException(nameof(actionInitiatorUserEmail));
        ActionInitiatorUserPhotoUrl = actionInitiatorUserPhotoUrl;
        FriendToRemoveUserId = friendToRemoveUserId ?? throw new ArgumentNullException(nameof(friendToRemoveUserId));
        FriendToRemoveUserEmail = friendToRemoveUserEmail ?? throw new ArgumentNullException(nameof(friendToRemoveUserEmail));
        FriendToRemoveUserPhotoUrl = friendToRemoveUserPhotoUrl;
    }

    public string ActionInitiatorUserId { get; set; }
    public string ActionInitiatorUserEmail { get; set; }
    public string? ActionInitiatorUserPhotoUrl { get; set; }

    public string FriendToRemoveUserId { get; set; }
    public string FriendToRemoveUserEmail { get; set; }
    public string? FriendToRemoveUserPhotoUrl { get; set; }
}
public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;

    public FriendRequestRemovedEventHandler(IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext;
    }
    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        await _messagesHubContext.Clients.Users(request.FriendToRemoveUserId).SendAsync("FriendRequestRemoved", request.ActionInitiatorUserEmail);
        return;
    }
}
