using MediatR;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace SignalR.Events.Handlers
{
    public class NotificationEventHandler : IRequestHandler<NotificationEvent>
    {

        private readonly IHubContext<PresenceHub> _presenceHubContext;

        public NotificationEventHandler(IHubContext<PresenceHub> presenceHubContext)
        {
            this._presenceHubContext = presenceHubContext;
        }
        public async Task Handle(NotificationEvent request, CancellationToken cancellationToken)
        {
            await _presenceHubContext.Clients.User(request.AppNotification.UserId).SendAsync("NewNotificationReceived", request.AppNotification);
        }
    }
}
