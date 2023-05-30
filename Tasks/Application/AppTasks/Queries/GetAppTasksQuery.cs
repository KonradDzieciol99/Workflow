using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Models.Dtos;

namespace Tasks.Application.AppTasks.Queries;

public record GetAppTasksQuery(string ProjectId, int Skip, int Take, string? OrderBy, bool? IsDescending, string? Filter, string? GroupBy, string? Search, string[]? SelectedColumns) : IAuthorizationRequest<List<AppTaskDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new List<IAuthorizationRequirement> { new ProjectMembershipRequirement(ProjectId) };
}


