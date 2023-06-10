using MediatR;
using MessageBus;
using MessageBus.Events;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers
{
    public class NewUserRegistrationEventHandler : IRequestHandler<NewUserRegistrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public NewUserRegistrationEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
        }
        public async Task Handle(NewUserRegistrationEvent request, CancellationToken cancellationToken)
        {

            var notification = new AppNotification(/*request.Id,*/
                                                   request.UserId,
                                                   "WelcomeNotification",
                                                   request.MessageCreated,
                                                   $"Thank you for registering {request.UserEmail}, have fun testing!",
                                                   "Workflow",
                                                   null,
                                                   null);

            _unitOfWork.AppNotificationRepository.Add(notification);

            if (await _unitOfWork.Complete()!)
                throw new InvalidOperationException();

            var @event= new NotificationAddedEvent(notification.Id,
                                                   notification.UserId,
                                                   //notification.ObjectId,
                                                   notification.NotificationType,
                                                   notification.CreationDate,
                                                   notification.Displayed,
                                                   notification.Description,
                                                   notification.NotificationPartnerId,
                                                   notification.NotificationPartnerEmail,
                                                   notification.NotificationPartnerPhotoUrl);

            await _azureServiceBusSender.PublishMessage(@event);

            return;
        }
    }
}
