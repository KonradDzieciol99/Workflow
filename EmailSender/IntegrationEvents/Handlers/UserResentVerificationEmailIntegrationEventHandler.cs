using MediatR;

namespace EmailSender.IntegrationEvents.Handlers;

public class UserResentVerificationEmailIntegrationEventHandler : IRequestHandler<UserResentVerificationEmailIntegrationEvent>
{
    private readonly Sender.ISender _emailSender;

    public UserResentVerificationEmailIntegrationEventHandler(Sender.ISender emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task Handle(UserResentVerificationEmailIntegrationEvent request, CancellationToken cancellationToken)
    {
        await _emailSender.CreateConfirmEmailMessage(request.UserEmail, request.VerificationToken, request.UserId);

        return;
    }
}