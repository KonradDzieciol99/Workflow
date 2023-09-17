using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Domain.Common.Exceptions;

namespace Projects.Application.ProjectMembers.Commands;



public record AcceptProjectInvitationCommand(string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new()
    {
        new ProjectMembershipRequirement(ProjectId),
    };
}
public class AcceptProjectInvitationCommandHandler : IRequestHandler<AcceptProjectInvitationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AcceptProjectInvitationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }
    public async Task Handle(AcceptProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId) ?? throw new ProjectDomainException("Such a project does not exist");

        project.AcceptInvitation(_currentUserService.GetUserId());

        await _unitOfWork.Complete();

        return;
    }
}
