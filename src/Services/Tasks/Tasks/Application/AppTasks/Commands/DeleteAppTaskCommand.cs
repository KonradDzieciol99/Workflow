using AutoMapper;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.Common.Exceptions;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Common.Exceptions;
using Tasks.Domain.Services;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;

namespace Tasks.Application.AppTasks.Commands;

public record DeleteAppTaskCommand(string Id, string ProjectId) : IAuthorizationRequest
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
public class DeleteAppTaskCommandHandler : IRequestHandler<DeleteAppTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IEventBusSender _azureServiceBusSender;
    private readonly IAppTaskService _appTaskService;

    public DeleteAppTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IEventBusSender messageBus, IAppTaskService appTaskService)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        this._azureServiceBusSender = messageBus ?? throw new ArgumentNullException(nameof(_azureServiceBusSender));
        this._appTaskService = appTaskService ?? throw new ArgumentNullException(nameof(appTaskService)); ;
        ;
    }
    public async Task Handle(DeleteAppTaskCommand request, CancellationToken cancellationToken)
    {

        var task = await _unitOfWork.AppTaskRepository.GetAsync(request.Id) ?? throw new TaskDomainException("Task cannot be found.",new NotFoundException());

        var projectMember = await _unitOfWork.ProjectMemberRepository.GetAsync(_currentUserService.GetUserId(), request.ProjectId) ?? throw new TaskDomainException("Project Member cannot be found.");

        _appTaskService.RemoveAppTask(task, projectMember);

        await _unitOfWork.Complete();

        await _azureServiceBusSender.PublishMessage(new TaskDeletedEvent(task.Id));

        return;

    }
}