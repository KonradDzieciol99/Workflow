using MediatR;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace SignalR.Events.Handlers
{
    public class NotificationAddedEventHandler : IRequestHandler<NotificationAddedEvent>
    {

        private readonly IHubContext<PresenceHub> _presenceHubContext;

        public NotificationAddedEventHandler(IHubContext<PresenceHub> presenceHubContext)
        {
            this._presenceHubContext = presenceHubContext;
        }
        public async Task Handle(NotificationAddedEvent request, CancellationToken cancellationToken)
        {
            //TODO zrobić DTO z NotificationAddedEvent
            await _presenceHubContext.Clients.User(request.UserId).SendAsync("NewNotificationReceived", request);
        }
    }
}
