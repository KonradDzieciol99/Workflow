﻿using AutoMapper;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Exceptions;
using Tasks.Application.Common.Models;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Common.Exceptions;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;

namespace Tasks.Application.AppTasks.Commands;

public record UpdateAppTaskCommand(string Id,
                                   string Name,
                                   string? Description,
                                   string ProjectId,
                                   string? TaskAssigneeMemberId,
                                   Priority? Priority,
                                   State? State,
                                   DateTime? DueDate,
                                   DateTime? StartDate,
                                   string? TaskLeaderId) : IAuthorizationRequest<AppTaskDto>
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

public class UpdateAppTaskCommandHandler : IRequestHandler<UpdateAppTaskCommand, AppTaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public UpdateAppTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IAzureServiceBusSender messageBus)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        this._azureServiceBusSender = messageBus ?? throw new ArgumentNullException(nameof(_azureServiceBusSender)); ;
    }
    public async Task<AppTaskDto> Handle(UpdateAppTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.AppTaskRepository.GetAsync(request.Id) ?? throw new TaskDomainException("Task cannot be found.",new NotFoundException());

        task.UpdateTask(request.Name,
                        request.Description,
                        request.TaskAssigneeMemberId,
                        request.Priority,
                        request.State,
                        request.DueDate,
                        request.StartDate,
                        request.TaskLeaderId);

        await _unitOfWork.Complete();

        await _azureServiceBusSender.PublishMessage(new TaskDeletedEvent(task.Id));

        return _mapper.Map<AppTaskDto>(task);
    }
}