using MessageBus;

namespace Notification.Application.IntegrationEvents;

public record NotificationAddedEvent(string Id, string UserId, int NotificationType, DateTime CreationDate, bool Displayed, string Description, string? NotificationPartnerId, string? NotificationPartnerEmail, string? NotificationPartnerPhotoUrl, List<string>? OldNotificationsIds) : IntegrationEvent;