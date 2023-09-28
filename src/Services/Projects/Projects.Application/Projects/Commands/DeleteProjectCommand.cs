using AutoMapper;
using HttpMessage.Authorization;
using HttpMessage.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.Projects.Commands;

public record DeleteProjectCommand(string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new()
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectAuthorRequirement(ProjectId)
        };
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.RemoveProject();

        _unitOfWork.ProjectRepository.Remove(project);

        await _unitOfWork.Complete();
    }
}
