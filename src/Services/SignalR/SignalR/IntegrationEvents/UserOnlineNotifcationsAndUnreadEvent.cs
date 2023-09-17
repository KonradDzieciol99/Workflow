using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public record UserOnlineNotifcationsAndUnreadEvent(UserDto OnlineUser, List<AppNotification> AppNotifications, int TotalCount, List<string> UnreadIds) : IntegrationEvent;

public class UserOnlineNotifcationsAndUnreadEventHandler : IRequestHandler<UserOnlineNotifcationsAndUnreadEvent>
{

    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public UserOnlineNotifcationsAndUnreadEventHandler(IHubContext<PresenceHub> presenceHubContext)
    {
        _presenceHubContext = presenceHubContext ?? throw new ArgumentNullException(nameof(presenceHubContext)); 
    }

    public async Task Handle(UserOnlineNotifcationsAndUnreadEvent request, CancellationToken cancellationToken)
    {

        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveNotifications", new PagedAppNotifications(request.AppNotifications, request.TotalCount), cancellationToken: cancellationToken);
        await _presenceHubContext.Clients.User(request.OnlineUser.Id).SendAsync("ReceiveUnreadNotifications", request.UnreadIds, cancellationToken: cancellationToken);

        return;
    }
}
