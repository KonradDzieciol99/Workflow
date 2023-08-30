using EmailEmitter.Sender;
using MediatR;

namespace EmailEmitter.IntegrationEvents.Handlers;

public class RegistrationEventHandler : IRequestHandler<RegistrationEvent>
{
    private readonly ISenderSource _emailSender;

    public RegistrationEventHandler(ISenderSource emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task Handle(RegistrationEvent request, CancellationToken cancellationToken)
    {
        await _emailSender.CreateConfirmEmailMessage(request.Email, request.Token, request.UserId);

        return;
    }
}
