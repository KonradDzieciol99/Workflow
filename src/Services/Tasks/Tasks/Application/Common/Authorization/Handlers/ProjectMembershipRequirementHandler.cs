using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.Common.Authorization.Handlers;

public class ProjectMembershipRequirementHandler
    : AuthorizationHandler<ProjectMembershipRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMembershipRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectMembershipRequirement requirement
    )
    {
        var userId =
            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new ArgumentNullException(nameof(context));
        var projectId =
            requirement.ProjectId ?? throw new ArgumentNullException(nameof(requirement));

        var result = await _unitOfWork.ProjectMemberRepository.CheckIfUserIsAMemberOfProject(
            projectId,
            userId
        );

        if (result)
            context.Succeed(requirement);

        await Task.CompletedTask;
        return;
    }
}
