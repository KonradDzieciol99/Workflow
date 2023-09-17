using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Application.Common.Authorization.Handlers;

public class ProjectManagementRequirementHandler : AuthorizationHandler<ProjectManagementRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectManagementRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectManagementRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentNullException(nameof(context));
        var projectId = requirement.ProjectId ?? throw new ArgumentNullException(nameof(requirement));

        var result = await _unitOfWork.ReadOnlyProjectMemberRepository.CheckIfUserHasRightsToMenageUserAsync(projectId, userId);

        if (result)
            context.Succeed(requirement);

        return;
    }
}
