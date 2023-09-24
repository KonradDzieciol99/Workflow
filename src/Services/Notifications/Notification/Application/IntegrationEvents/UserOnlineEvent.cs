using MessageBus;
using Notification.Application.Common.Models;
using Notification.Application.AppNotifications.Queries;
using Notification.Infrastructure.Repositories;
using Notification.Services;
using MediatR;

namespace Notification.Application.IntegrationEvents;

public record UserOnlineEvent(UserDto OnlineUser) : IntegrationEvent;

public class UserOnlineEventHandler : IRequestHandler<UserOnlineEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public UserOnlineEventHandler(IUnitOfWork unitOfWork, IEventBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }

    public async Task Handle(UserOnlineEvent request, CancellationToken cancellationToken)
    {
        var query = new GetAppNotificationsQuery(0, 5, null, null, null, null, null, null);

        var pagedNotifications = await _unitOfWork.AppNotificationRepository.GetAsync(
            request.OnlineUser.Id,
            query
        );

        var unreadIds = await _unitOfWork.AppNotificationRepository.GetUnreadAsync(
            request.OnlineUser.Id
        );

        var @event = new UserOnlineNotifcationsAndUnreadEvent(
            request.OnlineUser,
            pagedNotifications.AppNotifications,
            pagedNotifications.totalCount,
            unreadIds
        );

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
