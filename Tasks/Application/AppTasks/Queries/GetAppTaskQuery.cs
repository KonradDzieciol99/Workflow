using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Models.Dtos;

namespace Tasks.Application.AppTasks.Queries;

public record GetAppTaskQuery(string Id,string ProjectId) : IAuthorizationRequest<AppTaskDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(ProjectId),
        };
        return listOfRequirements;
    }

}
