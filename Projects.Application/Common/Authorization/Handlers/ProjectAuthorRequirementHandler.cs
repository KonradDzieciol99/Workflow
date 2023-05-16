﻿using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.ServiceInterfaces;
using System.Security.Claims;

namespace Projects.Application.Common.Authorization.Handlers
{
    public class ProjectAuthorRequirementHandler : AuthorizationHandler<ProjectAuthorRequirement>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectAuthorRequirementHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAuthorRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
            var projectId = requirement.ProjectId ?? throw new ArgumentNullException(nameof(context.Resource));

            var result = await _unitOfWork.ProjectMemberRepository.CheckIfUserIsALeaderAsync(projectId, userId);

            if (result)
                context.Succeed(requirement);

            await Task.CompletedTask;
            return;
        }
    }
}