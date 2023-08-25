using MediatR;
using MessageBus.Events;

namespace EmailSender.IntegrationEvents.Handlers;

public class RegistrationEventHandler : IRequestHandler<RegistrationEvent>
{
    private readonly Sender.ISender _emailSender;

    public RegistrationEventHandler(Sender.ISender emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task Handle(RegistrationEvent request, CancellationToken cancellationToken)
    {
        await _emailSender.CreateConfirmEmailMessage(request.Email, request.Token, request.UserId);

        return;
    }
}
