using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record ProjectMemberDeclineInvitationEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string ProjectIconUrl) : IntegrationEvent;
public class ProjectMemberDeclineInvitationEventHandler : IRequestHandler<ProjectMemberDeclineInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberDeclineInvitationEventHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task Handle(ProjectMemberDeclineInvitationEvent request, CancellationToken cancellationToken)
    {
        var oldNotifications = await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                                request.UserId,
                                request.ProjectId,
                                new List<NotificationType>()
                                {
                                    NotificationType.InvitationToProjectRecived,
                                });

        var notification = new AppNotification(request.UserId,
                                               NotificationType.InvitationToProjectDeclined,
                                               request.MessageCreated,
                                               $"You declined an invitation from {request.ProjectName}",
                                               request.ProjectId,
                                               request.ProjectName,
                                               request.ProjectIconUrl,
                                               true);

        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);
        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        return;
    }
}