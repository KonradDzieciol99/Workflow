using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.ProjectMembers.Commands;

public record UpdateProjectMemberCommand(ProjectMemberType Type, string ProjectId, string UserId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectManagementRequirement(ProjectId)
        };
        return listOfRequirements;
    }
}
public class UpdateProjectMemberCommandHandler : IRequestHandler<UpdateProjectMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.UpdateProjectMember(request.UserId, request.Type);

        await _unitOfWork.Complete();
    }
}


