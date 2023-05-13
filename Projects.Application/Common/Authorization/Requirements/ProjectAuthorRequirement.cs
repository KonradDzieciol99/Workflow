using Microsoft.AspNetCore.Authorization;

namespace Projects.Application.Common.Authorization.Requirements
{
    public class ProjectAuthorRequirement : IAuthorizationRequirement 
    {
        public string ProjectId { get; set; }
    }
}
