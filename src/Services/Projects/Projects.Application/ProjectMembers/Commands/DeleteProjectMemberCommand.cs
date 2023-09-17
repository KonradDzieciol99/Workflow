using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.ProjectMembers.Commands;

public record DeleteProjectMemberCommand(string MemberId, string ProjectId) : IAuthorizationRequest
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

public class DeleteProjectMemberCommandHandler : IRequestHandler<DeleteProjectMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.RemoveProjectMember(request.MemberId);

        await _unitOfWork.Complete();
    }
}


