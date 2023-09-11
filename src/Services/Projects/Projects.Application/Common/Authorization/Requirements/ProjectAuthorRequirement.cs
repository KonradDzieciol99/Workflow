using Microsoft.AspNetCore.Authorization;

namespace Projects.Application.Common.Authorization.Requirements;

public record ProjectAuthorRequirement(string ProjectId) : IAuthorizationRequirement;
