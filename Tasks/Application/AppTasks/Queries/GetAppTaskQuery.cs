using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;
using Tasks.Domain.Common.Exceptions;

namespace Tasks.Application.AppTasks.Queries;

public record GetAppTaskQuery(string Id,string ProjectId) : IAuthorizationRequest<AppTaskDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(ProjectId),
        };
        return listOfRequirements;
    }

}
public class GetAppTaskQueryHandler : IRequestHandler<GetAppTaskQuery, AppTaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAppTaskQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }
    public async Task<AppTaskDto> Handle(GetAppTaskQuery request, CancellationToken cancellationToken)
    {
        var appTask = await _unitOfWork.AppTaskRepository.GetAsync(request.Id);

        if (appTask == null)
            throw new TaskDomainException("Such a task does not exist");
        

        return _mapper.Map<AppTaskDto>(appTask);
    }
}