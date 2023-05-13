using Microsoft.AspNetCore.Authorization;

namespace Projects.Application.Common.Authorization.Requirements
{
    public class ProjectManagementRequirement : IAuthorizationRequirement 
    {
        public string ProjectId { get; set; }
    }
}
