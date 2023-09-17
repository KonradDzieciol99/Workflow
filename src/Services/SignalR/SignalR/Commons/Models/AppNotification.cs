namespace SignalR.Commons.Models;

public class AppNotification
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required int NotificationType { get; set; }
    public required DateTime CreationDate { get; set; }
    public required bool Displayed { get; set; }
    public required string Description { get; set; }
    public string? NotificationPartnerId { get; set; }
    public string? NotificationPartnerEmail { get; set; }
    public string? NotificationPartnerPhotoUrl { get; set; }
}
