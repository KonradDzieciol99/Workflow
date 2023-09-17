using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization.Requirements;
using Notification.Infrastructure.Repositories;
using System.Security.Claims;

namespace Notification.Application.Common.Authorization.Handlers;

public class ProjectMembershipRequirementHandler : AuthorizationHandler<NotificationOwnerRequirement>
{
    private readonly IUnitOfWork _unitOfWork;
    public ProjectMembershipRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotificationOwnerRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentNullException(nameof(context));
        var AppNotificationId = requirement.AppNotificationId ?? throw new ArgumentNullException(nameof(requirement));

        var result = await _unitOfWork.AppNotificationRepository.CheckIfUserIsAOwnerOfAppNotification(AppNotificationId, userId);

        if (result)
            context.Succeed(requirement);

        await Task.CompletedTask;
        return;
    }
}
