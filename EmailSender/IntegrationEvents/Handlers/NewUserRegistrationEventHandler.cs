using MediatR;
using MessageBus.Events;

namespace EmailSender.IntegrationEvents.Handlers;

public class NewUserRegistrationEventHandler : IRequestHandler<NewUserRegistrationEvent>
{
    private readonly Sender.ISender _emailSender;

    public NewUserRegistrationEventHandler(Sender.ISender emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task Handle(NewUserRegistrationEvent request, CancellationToken cancellationToken)
    {
        await _emailSender.CreateConfirmEmailMessage(request.UserEmail,request.Token, request.UserId);

        return;
    }
}
