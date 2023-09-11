using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Domain.Common.Enums;

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
    private readonly ICurrentUserService _currentUserService;

    public UpdateProjectMemberCommandHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        this._currentUserService = currentUserService;
    }
    public async Task Handle(UpdateProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.UpdateProjectMember(_currentUserService.GetUserId(), request.UserId, request.Type);

        await _unitOfWork.Complete();
    }
}


