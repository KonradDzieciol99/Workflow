using EmailEmitter.Sender;
using MediatR;
using MessageBus;

namespace EmailEmitter.IntegrationEvents;

public record RegistrationEvent(string Email, string Token, string UserId, string? PhotoUrl)
    : IntegrationEvent;

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
