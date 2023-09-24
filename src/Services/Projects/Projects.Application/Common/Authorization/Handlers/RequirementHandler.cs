using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Application.Common.Authorization.Handlers;

public class RequirementHandler : IAuthorizationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public RequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is ProjectAuthorRequirement)
            {
                var userId =
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new ArgumentNullException(nameof(context));
                var projectId =
                    context.Resource as string ?? throw new ArgumentNullException(nameof(context));

                context.Succeed(requirement);
            }
            else if (requirement is ProjectManagementRequirement)
            {
                var userId =
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new ArgumentNullException(nameof(context));
                var projectId =
                    context.Resource as string ?? throw new ArgumentNullException(nameof(context));

                var projectMember =
                    await _unitOfWork.ReadOnlyProjectMemberRepository.GetProjectMemberAsync(
                        projectId,
                        userId
                    );

                if (projectMember is null)
                {
                    context.Fail(new AuthorizationFailureReason(this, "Woopsy"));
                }

                context.Succeed(requirement);
            }
            else if (requirement is ProjectMembershipRequirement)
            {
                context.Succeed(requirement);
            }
        }

        await Task.CompletedTask;

        return;
    }
}
