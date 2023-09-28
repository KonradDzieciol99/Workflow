using AutoMapper;
using HttpMessage.Authorization;
using HttpMessage.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Projects.Queries;

public record GetProjectQuery(string ProjectId) : IAuthorizationRequest<ProjectDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new() { new ProjectMembershipRequirement(ProjectId) };
}

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProjectQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProjectDto> Handle(
        GetProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork.ReadOnlyProjectRepository.GetOneAsync(request.ProjectId)
            ?? throw new NotFoundException("Project cannot be found.");

        return _mapper.Map<ProjectDto>(project);
    }
}
