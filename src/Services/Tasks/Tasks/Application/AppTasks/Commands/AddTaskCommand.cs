using AutoMapper;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Exceptions;
using Tasks.Application.Common.Models;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;

namespace Tasks.Application.AppTasks.Commands;

public record AddTaskCommand(
    string Name,
    string? Description,
    string ProjectId,
    string? TaskAssigneeMemberId,
    Priority Priority,
    State State,
    DateTime DueDate,
    DateTime StartDate,
    string TaskLeaderId
) : IAuthorizationRequest<AppTaskDto>
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

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, AppTaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEventBusSender _messageBus;

    public AddTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventBusSender messageBus)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this._messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public async Task<AppTaskDto> Handle(
        AddTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var appTask = new AppTask(
            request.Name,
            request.Description,
            request.ProjectId,
            request.TaskAssigneeMemberId,
            request.Priority,
            request.State,
            request.DueDate,
            request.StartDate,
            request.TaskLeaderId
        );

        _unitOfWork.AppTaskRepository.Add(appTask);

        await _unitOfWork.Complete();

        var @event = new TaskAddedEvent(
            appTask.Id,
            appTask.Name,
            appTask.Description,
            appTask.ProjectId,
            appTask.TaskAssigneeMemberId,
            (int)appTask.Priority,
            (int)appTask.State,
            appTask.DueDate,
            appTask.StartDate,
            appTask.TaskLeaderId
        );

        await _messageBus.PublishMessage(@event);

        return _mapper.Map<AppTaskDto>(appTask);
    }
}
