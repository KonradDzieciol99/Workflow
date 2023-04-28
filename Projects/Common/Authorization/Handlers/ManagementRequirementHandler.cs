using Microsoft.AspNetCore.Authorization;
using Projects.Common.Authorization.Requirements;
using Projects.Entity;
using Projects.Repositories;
using System.Security.Claims;

namespace Projects.Common.Authorization.Handlers
{
    public class ManagementRequirementHandler : AuthorizationHandler<ManagementRequirement, ProjectMember>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManagementRequirementHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagementRequirement requirement, ProjectMember resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
            var projectId = context.Resource as string ?? throw new ArgumentNullException(nameof(context.Resource));

            var result = await _unitOfWork.ProjectMemberRepository.CheckIfUserHasRightsToMenageUserAsync(projectId, userId);

            if (result)
                context.Succeed(requirement);

            await Task.CompletedTask;
            return;
        }
    }
}
