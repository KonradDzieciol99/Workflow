using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;

namespace Projects.Application.ProjectMembers.Queries;

public record GetMembersStatusesQuery(string projectId, List<string> UsersIds) : IAuthorizationRequest<List<MemberStatusDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(projectId),
            new ProjectManagementRequirement(projectId)
        };
        return listOfRequirements;
    }
}
public class GetFriendsStatusQueryHandler : IRequestHandler<GetMembersStatusesQuery, List<MemberStatusDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFriendsStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<List<MemberStatusDto>> Handle(GetMembersStatusesQuery request, CancellationToken cancellationToken)
    {
        var membersStatuses = await _unitOfWork.ReadOnlyProjectMemberRepository.GetMembersStatusesAsync(request.projectId, request.UsersIds);
        return membersStatuses;
    }
}

//public async Task<List<FriendStatusDto>> Handle(GetFriendsStatusQuery request, CancellationToken cancellationToken)
//{
//    var friendRequests = await _unitOfWork.FriendRequestRepository.CheckUsersToUserStatusAsync(_currentUserService.GetUserId(), request.UsersIds);
//    return friendRequests;
//}