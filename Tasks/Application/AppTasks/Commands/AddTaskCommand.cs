using AutoMapper;
using MediatR;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Exceptions;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;

namespace Tasks.Application.AppTasks.Commands;

public record AddTaskCommand(string Name,
                             string? Description,
                             string ProjectId,
                             string? TaskAssigneeMemberId,
                             //string? TaskAssigneeMemberEmail,
                             //string? TaskAssigneeMemberPhotoUrl,
                             Priority Priority,
                             State State,
                             DateTime DueDate,
                             DateTime StartDate,
                             string TaskLeaderId) : IAuthorizationRequest<AppTaskDto>
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
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IAzureServiceBusSender _messageBus;

    public AddTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IAzureServiceBusSender messageBus)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        this._messageBus = messageBus;
    }
    public async Task<AppTaskDto> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var appTask = new AppTask(request.Name,
                                  request.Description,
                                  request.ProjectId,
                                  request.TaskAssigneeMemberId,
                                  request.Priority,
                                  request.State,
                                  request.DueDate,
                                  request.StartDate,
                                  request.TaskLeaderId);

        _unitOfWork.AppTaskRepository.Add(appTask);

        if (await _unitOfWork.Complete())
        {
            var @event = new TaskAddedEvent(appTask.Id,
                               appTask.Name,
                               appTask.Description,
                               appTask.ProjectId,
                               appTask.TaskAssigneeMemberId,
                               //appTask.TaskAssigneeMemberEmail,
                               //appTask.TaskAssigneeMemberPhotoUrl,
                               (int)appTask.Priority,
                               (int)appTask.State, appTask.DueDate, appTask.StartDate, appTask.TaskLeaderId);

            await _messageBus.PublishMessage(@event);

            return _mapper.Map<AppTaskDto>(appTask);
        }

        throw new BadRequestException("task could not be added.");
    }
}