using AutoMapper;
using HttpMessage.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Projects.Commands;

public record UpdateProjectCommand(
    string? Name,
    string? IconUrl,
    string? NewLeaderId,
    string ProjectId
) : IAuthorizationRequest<ProjectDto>
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

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProjectDto> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken
    )
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.Update(request.Name, request.IconUrl, request.NewLeaderId);

        await _unitOfWork.Complete();

        return _mapper.Map<ProjectDto>(project);
    }
}
