using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Projects.Queries;

public record GetProjectsQuery(int Skip, int Take, string? OrderBy, bool? IsDescending, string? Filter, string? GroupBy, string? Search, string[]? SelectedColumns) : IAuthorizationRequest<ProjectsWithTotalCount>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>();
}
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ProjectsWithTotalCount>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectsQueryHandler(IUnitOfWork unitOfWork,IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        this._mapper = mapper;
        this._currentUserService = currentUserService;
    }

    public async Task<ProjectsWithTotalCount> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ReadOnlyProjectMemberRepository.Get(_currentUserService.GetUserId(), request);
        
        var projectsWithTotalCount = new ProjectsWithTotalCount(result.TotalCount, _mapper.Map<List<ProjectDto>>(result.Projects));

        return projectsWithTotalCount;
    }

    //public async Task<ProjectsWithTotalCount> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    //{
    //    var result = await _unitOfWork.ReadOnlyProjectMemberRepository.Get(_currentUserService.UserId, request.AppParams);

    //    var projectsWithTotalCount = new ProjectsWithTotalCount()
    //    {
    //        Count = result.TotalCount,
    //        Result = _mapper.Map<List<ProjectDto>>(result.Projects)
    //    };

    //    return projectsWithTotalCount;
    //}
}


