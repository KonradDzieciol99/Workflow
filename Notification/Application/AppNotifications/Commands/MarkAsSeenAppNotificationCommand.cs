using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization;
using Notification.Application.Common.Authorization.Requirements;
using Notification.Infrastructure.Repositories;
using Notification.Domain.Common.Exceptions;
using Notification.Application.Common.Exceptions;

namespace Notification.Application.AppNotifications.Commands;

public record MarkAsSeenAppNotificationCommand(string Id) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new NotificationOwnerRequirement(Id),
        };
        return listOfRequirements;
    }
}

public class MarkAsSeenAppNotificationCommandHandler : IRequestHandler<MarkAsSeenAppNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsSeenAppNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
    }
    public async Task Handle(MarkAsSeenAppNotificationCommand request, CancellationToken cancellationToken)
    {

        var appNotification = await _unitOfWork.AppNotificationRepository.GetAsync(request.Id) ?? throw new NotificationDomainException(string.Empty, new BadRequestException("Notification cannot be found."));

        appNotification.MarkAsSeen();

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        return;
    }
}