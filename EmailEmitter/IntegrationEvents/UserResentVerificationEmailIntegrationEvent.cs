using EmailEmitter.Sender;
using MediatR;
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
public class UserResentVerificationEmailIntegrationEventHandler : IRequestHandler<UserResentVerificationEmailIntegrationEvent>
{
    private readonly ISenderSource _emailSender;

    public UserResentVerificationEmailIntegrationEventHandler(ISenderSource emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task Handle(UserResentVerificationEmailIntegrationEvent request, CancellationToken cancellationToken)
    {
        await _emailSender.CreateConfirmEmailMessage(request.UserEmail, request.VerificationToken, request.UserId);

        return;
    }
}