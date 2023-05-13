using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.ServiceInterfaces;
using System.Security.Claims;

namespace Projects.Application.Common.Authorization.Handlers
{
    public class ProjectMembershipRequirementHandler : AuthorizationHandler<ProjectMembershipRequirement, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectMembershipRequirementHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectMembershipRequirement requirement, string resource)
        {

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
            var projectId = context.Resource as string ?? throw new ArgumentNullException(nameof(context.Resource));

            var result = await _unitOfWork.ProjectMemberRepository.CheckIfUserIsAMemberOfProject(projectId, userId);

            if (result)
                context.Succeed(requirement);

            await Task.CompletedTask;
            return;
        }
    }
}
