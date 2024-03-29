﻿using HttpMessage.Authorization;
using HttpMessage.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization.Requirements;
using Notification.Domain.Common.Exceptions;
using Notification.Infrastructure.Repositories;

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

public class MarkAsSeenAppNotificationCommandHandler
    : IRequestHandler<MarkAsSeenAppNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsSeenAppNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(
        MarkAsSeenAppNotificationCommand request,
        CancellationToken cancellationToken
    )
    {
        var appNotification =
            await _unitOfWork.AppNotificationRepository.GetAsync(request.Id)
            ?? throw new NotFoundException("Notification cannot be found.");

        appNotification.MarkAsSeen();

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        return;
    }
}
