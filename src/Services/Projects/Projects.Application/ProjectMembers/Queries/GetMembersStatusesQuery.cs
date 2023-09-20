using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;

namespace Projects.Application.ProjectMembers.Queries;

public record GetMembersStatusesQuery(string ProjectId, List<string> UsersIds) : IAuthorizationRequest<List<MemberStatusDto>>
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
public class GetFriendsStatusQueryHandler : IRequestHandler<GetMembersStatusesQuery, List<MemberStatusDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFriendsStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<List<MemberStatusDto>> Handle(GetMembersStatusesQuery request, CancellationToken cancellationToken)
    {
        var membersStatuses = await _unitOfWork.ReadOnlyProjectMemberRepository.GetMembersStatusesAsync(request.ProjectId, request.UsersIds);
        return membersStatuses;
    }
}