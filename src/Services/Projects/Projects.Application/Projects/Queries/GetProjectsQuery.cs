using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Projects.Queries;

public record GetProjectsQuery(
    int Skip,
    int Take,
    string? OrderBy,
    bool? IsDescending,
    string? Filter,
    string? GroupBy,
    string? Search,
    string[]? SelectedColumns
) : IAuthorizationRequest<ProjectsWithTotalCount>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ProjectsWithTotalCount>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this._currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<ProjectsWithTotalCount> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await _unitOfWork.ReadOnlyProjectMemberRepository.Get(
            _currentUserService.GetUserId(),
            request
        );

        var projectsWithTotalCount = new ProjectsWithTotalCount(
            result.TotalCount,
            _mapper.Map<List<ProjectDto>>(result.Projects)
        );

        return projectsWithTotalCount;
    }
}
