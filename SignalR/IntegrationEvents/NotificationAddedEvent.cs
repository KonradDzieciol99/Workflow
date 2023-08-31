using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public class NotificationAddedEvent : IntegrationEvent
{
    public NotificationAddedEvent(string id, string userId, int notificationType, DateTime creationDate, bool displayed, string description, string? notificationPartnerId, string? notificationPartnerEmail, string? notificationPartnerPhotoUrl, List<string>? oldNotificationsIds)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        NotificationType = notificationType;
        CreationDate = creationDate;
        Displayed = displayed;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        NotificationPartnerId = notificationPartnerId;
        NotificationPartnerEmail = notificationPartnerEmail;
        NotificationPartnerPhotoUrl = notificationPartnerPhotoUrl;
        OldNotificationsIds = oldNotificationsIds;
    }

    public string Id { get; set; }
    public string UserId { get; set; }
    public int NotificationType { get; set; }
    public DateTime CreationDate { get; set; }
    public bool Displayed { get; set; }
    public string Description { get; set; }
    public string? NotificationPartnerId { get; set; }
    public string? NotificationPartnerEmail { get; set; }
    public string? NotificationPartnerPhotoUrl { get; set; }
    public List<string>? OldNotificationsIds { get; set; }
}
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
