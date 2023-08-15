using MediatR;
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



public record AcceptProjectInvitationCommand(string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>()
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
        this._unitOfWork = unitOfWork;
        this._currentUserService = currentUserService;
    }
    public async Task Handle(AcceptProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.AcceptInvitation(_currentUserService.GetUserId());

        await _unitOfWork.Complete();

        return;
    }
}
