using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.Projects.Commands;

public record UpdateProjectCommand(string? Name, string? IconUrl, string? NewLeaderId, string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectAuthorRequirement(ProjectId)
        };
    }
}
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.Update(request.Name, request.IconUrl, request.NewLeaderId);

        await _unitOfWork.Complete();
    }
}