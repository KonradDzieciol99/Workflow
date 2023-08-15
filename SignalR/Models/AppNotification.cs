namespace SignalR.Models;

public class AppNotification
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public int NotificationType { get; set; }
    public DateTime CreationDate { get; set; }
    public bool Displayed { get; set; }
    public string Description { get; set; }
    public string? NotificationPartnerId { get; set; }
    public string? NotificationPartnerEmail { get; set; }
    public string? NotificationPartnerPhotoUrl { get; set; }
}
