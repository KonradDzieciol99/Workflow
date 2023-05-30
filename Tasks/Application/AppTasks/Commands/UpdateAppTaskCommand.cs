using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Models.Dtos;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;
using Tasks.Application.Common.Exceptions;

namespace Tasks.Application.AppTasks.Commands;

public record UpdateAppTaskCommand(AppTaskDto AppTaskDto) : IAuthorizationRequest<AppTaskDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(AppTaskDto.ProjectId),
        };
        return listOfRequirements;
    }
}

public class UpdateAppTaskCommandHandler : IRequestHandler<UpdateAppTaskCommand,AppTaskDto>
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

        var task = await _unitOfWork.AppTaskRepository.Get(request.AppTaskDto.Id) ?? throw new BadRequestException("Task cannot be found.");

        task.UpdateTask(request.AppTaskDto.Name,
                        request.AppTaskDto.Description,
                        request.AppTaskDto.TaskAssigneeMemberId,
                        request.AppTaskDto.TaskAssigneeMemberEmail,
                        request.AppTaskDto.TaskAssigneeMemberPhotoUrl,
                        request.AppTaskDto.Priority,
                        request.AppTaskDto.State,
                        request.AppTaskDto.DueDate,
                        request.AppTaskDto.StartDate,
                        request.AppTaskDto.TaskLeaderId);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        await _azureServiceBusSender.PublishMessage(new TaskDeletedEvent(task.Id));

        return _mapper.Map<AppTaskDto>(task);
    }
}