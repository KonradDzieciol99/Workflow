using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Application.Common.Authorization.Handlers;

public class ProjectAuthorRequirementHandler : AuthorizationHandler<ProjectAuthorRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectAuthorRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectAuthorRequirement requirement
    )
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var userId =
            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException($"Claim ({nameof(ClaimTypes.NameIdentifier)}) is missing in the context.");
        var projectId =
            requirement.ProjectId ?? throw new InvalidOperationException("The project identifier is missing in the requirement.");

        var result = await _unitOfWork.ReadOnlyProjectMemberRepository.CheckIfUserIsALeaderAsync(
            projectId,
            userId
        );

        if (result)
            context.Succeed(requirement);

        await Task.CompletedTask;
        return;
    }
}
