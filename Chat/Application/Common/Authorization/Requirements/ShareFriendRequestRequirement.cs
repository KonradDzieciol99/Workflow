using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Common.Authorization.Requirements;

public record ShareFriendRequestRequirement(string TargetUserId) : IAuthorizationRequirement {}
