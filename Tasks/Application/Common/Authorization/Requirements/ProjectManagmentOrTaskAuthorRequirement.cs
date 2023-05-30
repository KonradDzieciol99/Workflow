using Microsoft.AspNetCore.Authorization;

namespace Tasks.Application.Common.Authorization.Requirements
{
    public class ProjectManagmentOrTaskAuthorRequirement : IAuthorizationRequirement
    {
        public ProjectManagmentOrTaskAuthorRequirement(string projectId, string appTaskId)
        {
            ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
            AppTaskId = appTaskId ?? throw new ArgumentNullException(nameof(appTaskId));
        }

        public string ProjectId { get; set; }
        public string AppTaskId { get; set; }
    }
}
