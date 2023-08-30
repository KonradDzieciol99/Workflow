using MessageBus;

namespace EmailEmitter.IntegrationEvents;

public class UserResentVerificationEmailIntegrationEvent : IntegrationEvent
{
    public UserResentVerificationEmailIntegrationEvent(string userEmail, string verificationToken, string userId)
    {
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        VerificationToken = verificationToken ?? throw new ArgumentNullException(nameof(verificationToken));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    public string UserEmail { get; set; }
    public string VerificationToken { get; set; }
    public string UserId { get; set; }
}
