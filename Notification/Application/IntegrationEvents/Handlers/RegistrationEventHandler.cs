using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers;

public class RegistrationEventHandler : IRequestHandler<RegistrationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public RegistrationEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
    }
    public async Task Handle(RegistrationEvent request, CancellationToken cancellationToken)
    {

        var notification = new AppNotification(request.UserId,
                                               NotificationType.WelcomeNotification,
                                               request.MessageCreated,
                                               $"Thank you for registering {request.Email}, have fun testing!",
                                               "Workflow",
                                               null,
                                               null);

        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var @event = new NotificationAddedEvent(notification.Id,
                                               notification.UserId,
                                               (int)notification.NotificationType,
                                               notification.CreationDate,
                                               notification.Displayed,
                                               notification.Description,
                                               notification.NotificationPartnerId,
                                               notification.NotificationPartnerEmail,
                                               notification.NotificationPartnerPhotoUrl,
                                               null);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
