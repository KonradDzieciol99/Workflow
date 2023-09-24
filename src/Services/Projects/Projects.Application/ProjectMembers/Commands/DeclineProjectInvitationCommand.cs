using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.ProjectMembers.Commands;

public record DeclineProjectInvitationCommand(string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new() { new ProjectMembershipRequirement(ProjectId), };
}

public class DeclineProjectInvitationCommandHandler
    : IRequestHandler<DeclineProjectInvitationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeclineProjectInvitationCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task Handle(
        DeclineProjectInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.DeclineInvitation(_currentUserService.GetUserId());

        await _unitOfWork.Complete();

        return;
    }
}
