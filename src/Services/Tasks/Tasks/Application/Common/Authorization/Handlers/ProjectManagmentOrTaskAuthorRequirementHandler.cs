using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.Common.Authorization.Handlers;

public class ProjectManagmentOrTaskAuthorRequirementHandler
    : AuthorizationHandler<ProjectManagmentOrTaskAuthorRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectManagmentOrTaskAuthorRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectManagmentOrTaskAuthorRequirement requirement
    )
    {
        var userId =
            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new ArgumentNullException(nameof(context));
        var projectId =
            requirement.ProjectId ?? throw new ArgumentNullException(nameof(requirement));
        var appTaskId =
            requirement.AppTaskId ?? throw new ArgumentNullException(nameof(requirement));

        var result = await _unitOfWork.AppTaskRepository.CheckIfUserHasRightsToMenageTaskAsync(
            projectId,
            userId,
            appTaskId
        );

        if (result)
            context.Succeed(requirement);

        return;
    }
}
