using Microsoft.AspNetCore.Authorization;

namespace Projects.Application.Common.Authorization.Requirements
{
    public class ProjectMembershipRequirement : IAuthorizationRequirement
    {
        public string ProjectId { get; set; }
    }
}
