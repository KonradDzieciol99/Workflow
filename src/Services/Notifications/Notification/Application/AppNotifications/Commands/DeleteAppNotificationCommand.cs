using HttpMessage.Authorization;
using HttpMessage.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization.Requirements;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.AppNotifications.Commands;

public record DeleteAppNotificationCommand(string Id) : IAuthorizationRequest
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

public class DeleteAppNotificationCommandHandler : IRequestHandler<DeleteAppNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAppNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(
        DeleteAppNotificationCommand request,
        CancellationToken cancellationToken
    )
    {
        var appNotification =
            await _unitOfWork.AppNotificationRepository.GetAsync(request.Id)
            ?? throw new NotFoundException("Notification cannot be found.");

        _unitOfWork.AppNotificationRepository.Remove(appNotification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        return;
    }
}
