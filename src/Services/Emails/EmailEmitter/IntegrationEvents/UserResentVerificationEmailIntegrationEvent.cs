using EmailEmitter.Sender;
using MediatR;
using MessageBus;

namespace EmailEmitter.IntegrationEvents;

public record UserResentVerificationEmailIntegrationEvent(
    string UserEmail,
    string VerificationToken,
    string UserId
) : IntegrationEvent;

public class UserResentVerificationEmailIntegrationEventHandler
    : IRequestHandler<UserResentVerificationEmailIntegrationEvent>
{
    private readonly ISenderSource _emailSender;

    public UserResentVerificationEmailIntegrationEventHandler(ISenderSource emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Handle(
        UserResentVerificationEmailIntegrationEvent request,
        CancellationToken cancellationToken
    )
    {
        await _emailSender.CreateConfirmEmailMessage(
            request.UserEmail,
            request.VerificationToken,
            request.UserId
        );

        return;
    }
}
