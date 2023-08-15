using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Messages.Queries;

public class GetUnreadMessagesCountQuery : IAuthorizationRequest<int>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>();
    
}