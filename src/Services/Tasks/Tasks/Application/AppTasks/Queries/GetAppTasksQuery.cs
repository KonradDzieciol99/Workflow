using AutoMapper;
using HttpMessage.Authorization;
using HttpMessage.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.AppTasks.Queries;

public record GetAppTasksQuery(
    string ProjectId,
    int Skip,
    int Take,
    string? OrderBy,
    bool? IsDescending,
    string? Filter,
    string? GroupBy,
    string? Search,
    string[]? SelectedColumns
) : IAuthorizationRequest<AppTaskDtosWithTotalCount>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new() { new ProjectMembershipRequirement(ProjectId) };
}

public class GetAppTasksQueryHandler : IRequestHandler<GetAppTasksQuery, AppTaskDtosWithTotalCount>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetAppTasksQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this._currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<AppTaskDtosWithTotalCount> Handle(
        GetAppTasksQuery request,
        CancellationToken cancellationToken
    )
    {
        var (appTasks, totalCount) = await _unitOfWork.AppTaskRepository.GetAsync(
            _currentUserService.GetUserId(),
            request
        );

        var appTasksWithTotalCount = new AppTaskDtosWithTotalCount(
            totalCount,
            _mapper.Map<List<AppTaskDto>>(appTasks)
        );

        return appTasksWithTotalCount;
    }
}
