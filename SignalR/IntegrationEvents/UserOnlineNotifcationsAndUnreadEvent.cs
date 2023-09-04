using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public class UserOnlineNotifcationsAndUnreadEvent : IntegrationEvent
{
    public UserDto OnlineUser { get; set; }
    public List<AppNotification> AppNotifications { get; set; }
    public int TotalCount { get; set; }
    public List<string> UnreadIds { get; set; }

}
public class UserOnlineNotifcationsAndUnreadEventHandler : IRequestHandler<UserOnlineNotifcationsAndUnreadEvent>
{

    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public UserOnlineNotifcationsAndUnreadEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        _presenceHubContext = presenceHubContext;
    }

    public async Task Handle(UserOnlineNotifcationsAndUnreadEvent request, CancellationToken cancellationToken)
    {

        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveNotifications", new PagedAppNotifications(request.AppNotifications, request.TotalCount));
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveUnreadNotifications", request.UnreadIds);

        return;
    }
}
