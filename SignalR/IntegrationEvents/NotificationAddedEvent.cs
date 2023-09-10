using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public record NotificationAddedEvent(string Id, string UserId, int NotificationType, DateTime CreationDate, bool Displayed, string Description, string? NotificationPartnerId, string? NotificationPartnerEmail, string? NotificationPartnerPhotoUrl, List<string>? OldNotificationsIds) : IntegrationEvent;
public class NotificationAddedEventHandler : IRequestHandler<NotificationAddedEvent>
{

    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public NotificationAddedEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(NotificationAddedEvent request, CancellationToken cancellationToken)
    {
        //TODO zrobić DTO z NotificationAddedEvent
        await _presenceHubContext.Clients.User(request.UserId).SendAsync("NewNotificationReceived", request);
    }
}
