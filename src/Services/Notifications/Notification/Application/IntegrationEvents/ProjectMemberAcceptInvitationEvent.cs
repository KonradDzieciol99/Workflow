using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record ProjectMemberAcceptInvitationEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string projectIconUrl) : IntegrationEvent;
public class ProjectMemberAcceptInvitationEventHandler : IRequestHandler<ProjectMemberAcceptInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberAcceptInvitationEventHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task Handle(ProjectMemberAcceptInvitationEvent request, CancellationToken cancellationToken)
    {
        var oldNotifications = await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                                request.UserId,
                                request.ProjectId,
                                new List<NotificationType>()
                                {
                                    NotificationType.InvitationToProjectRecived,
                                });

        var notification = new AppNotification(request.UserId,
                                               NotificationType.InvitationToProjectAccepted,
                                               request.MessageCreated,
                                               $"you have accepted an invitation to {request.ProjectName}",
                                               request.ProjectId,
                                               request.ProjectName,
                                               request.projectIconUrl,
                                               true);

        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);
        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        return;
    }
}
