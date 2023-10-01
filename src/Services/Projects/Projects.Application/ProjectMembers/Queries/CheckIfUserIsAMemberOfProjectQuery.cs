using HttpMessage.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.ProjectMembers.Queries;
public record CheckIfUserIsAMemberOfProjectQuery(string ProjectId, string UserId) : IAuthorizationRequest<bool>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>();
    }
}
public class CheckIfUserIsAMemberOfProjectQueryHandler : IRequestHandler<CheckIfUserIsAMemberOfProjectQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckIfUserIsAMemberOfProjectQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(
        CheckIfUserIsAMemberOfProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork.ReadOnlyProjectMemberRepository.CheckIfUserIsAMemberOfProject(request.ProjectId,request.UserId);
    }
}
