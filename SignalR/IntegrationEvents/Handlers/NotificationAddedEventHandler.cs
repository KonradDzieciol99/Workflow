using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

public class NotificationAddedEventHandler : IRequestHandler<NotificationAddedEvent>
{

    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public NotificationAddedEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(NotificationAddedEvent request, CancellationToken cancellationToken)
    {
        //TODO zrobić DTO z NotificationAddedEvent
        await _presenceHubContext.Clients.User(request.UserId).SendAsync("NewNotificationReceived", request);
    }
}
