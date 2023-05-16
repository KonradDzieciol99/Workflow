using Microsoft.AspNetCore.Authorization;
using Projects.Common.Authorization.Requirements;
using Projects.Entity;
using Projects.Repositories;
using System.Security.Claims;

namespace Projects.Common.Authorization.Handlers
{
    public class RequirementHandler : IAuthorizationHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequirementHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                if (requirement is AuthorRequirement)
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
                    var projectId = context.Resource as string ?? throw new ArgumentNullException(nameof(context.Resource));
                    //mozna lepiej


                    context.Succeed(requirement);

                    // if (projectMember is not null && projectMember.Type == ProjectMemberType.Leader)


                    //context.User, context.Resource
                }
                else if (requirement is ManagementRequirement)
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
                    var projectId = context.Resource as string ?? throw new ArgumentNullException(nameof(context.Resource));

                    var projectMember = await _unitOfWork.ProjectMemberRepository.GetProjectMemberAsync(projectId, userId);

                    if (projectMember is null)
                    {
                        context.Fail(new AuthorizationFailureReason(this, "Woopsy"));
                    }

                    context.Succeed(requirement);
                }
                else if (requirement is MembershipRequirement)
                {
                    context.Succeed(requirement);
                }
            }

            await Task.CompletedTask;

            return;
        }

    }
}
