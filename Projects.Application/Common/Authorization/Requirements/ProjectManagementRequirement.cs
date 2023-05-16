using Microsoft.AspNetCore.Authorization;

namespace Projects.Application.Common.Authorization.Requirements
{
    public class ProjectManagementRequirement : IAuthorizationRequirement 
    {
        public ProjectManagementRequirement(string projectId)
        {
            ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        }

        public string ProjectId { get; set; }
    }
}
