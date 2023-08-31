using EmailEmitter.Sender;
using MediatR;
using MessageBus;

namespace EmailEmitter.IntegrationEvents;

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

