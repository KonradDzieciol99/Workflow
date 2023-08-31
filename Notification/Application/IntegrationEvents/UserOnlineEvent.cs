using MessageBus;
using Notification.Application.Common.Models;
using Notification.Application.AppNotifications.Queries;
using Notification.Infrastructure.Repositories;
using Notification.Services;
using MediatR;

namespace Notification.Application.IntegrationEvents;


public class UserOnlineEvent : IntegrationEvent
{
    public UserOnlineEvent(UserDto onlineUser)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
    }

    public UserDto OnlineUser { get; set; }
}
public class UserOnlineEventHandler : IRequestHandler<UserOnlineEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public UserOnlineEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAzureServiceBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._currentUserService = currentUserService;
        this._azureServiceBusSender = azureServiceBusSender;
    }
    public async Task Handle(UserOnlineEvent request, CancellationToken cancellationToken)
    {
        var query = new GetAppNotificationsQuery(0, 5, null, null, null, null, null, null);

        var pagedNotifications = await _unitOfWork.AppNotificationRepository.GetAsync(request.OnlineUser.Id, query);

        var unreadIds = await _unitOfWork.AppNotificationRepository.GetUnreadAsync(request.OnlineUser.Id);

        var @event = new UserOnlineNotifcationsAndUnreadEvent(request.OnlineUser, pagedNotifications.AppNotifications, pagedNotifications.totalCount, unreadIds);

        await _azureServiceBusSender.PublishMessage(@event);

        return;

        //odczytać jeszcze ilość nieprzecz
        // a może zrobić tak że sprawdzać tylko ile jest nieodzytanych i na przykład zwracać id nieodczytanych notyfikacji a..
        // a notyfikacje w samie w sobie będzie pobierać.. w innym serwisie
        // nie to nie jest doby pommysł bo jak będą przychodziły nowe notyfikacje to włąsnie do tego serwisu a 
        // a na poczatek ma pobierać po prostu 5 ostatnich według Daty, jeśli użytkownik bedzie chiał będzie mógł sobie kliknąć ze
        //mają się pojawić nieprzeczytane w tedy pobiera nieprzeczytane TYLKO bez przecztanych tak jak na fb 
    }
}


