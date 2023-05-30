using Microsoft.AspNetCore.Authorization;

namespace Tasks.Application.Common.Authorization.Requirements
{
    public class ProjectMembershipRequirement : IAuthorizationRequirement
    {
        public ProjectMembershipRequirement(string projectId)
        {
            ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        }

        public string ProjectId { get; set; }
    }
}
