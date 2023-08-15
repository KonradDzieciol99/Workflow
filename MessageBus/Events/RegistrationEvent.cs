using System;

namespace MessageBus.Events;

public class RegistrationEvent : IntegrationEvent
{
    public RegistrationEvent(string email, string token, string userId, string? photoUrl)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        PhotoUrl = photoUrl;
    }

    public string Email { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public string? PhotoUrl { get; set; }
}
