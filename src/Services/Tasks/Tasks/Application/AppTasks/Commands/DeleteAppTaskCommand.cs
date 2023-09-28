using HttpMessage.Authorization;
using HttpMessage.Exceptions;
using HttpMessage.Services;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Services;
using Tasks.Infrastructure.Repositories;

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
    private readonly IEventBusSender _eventBusSender;
    private readonly IAppTaskService _appTaskService;

    public DeleteAppTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEventBusSender eventBusSender,
        IAppTaskService appTaskService
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._eventBusSender =
            eventBusSender ?? throw new ArgumentNullException(nameof(eventBusSender));
        this._appTaskService =
            appTaskService ?? throw new ArgumentNullException(nameof(appTaskService));
    }

    public async Task Handle(DeleteAppTaskCommand request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork.AppTaskRepository.GetAsync(request.Id)
            ?? throw new NotFoundException("Task cannot be found.");

        var projectMember =
            await _unitOfWork.ProjectMemberRepository.GetAsync(
                _currentUserService.GetUserId(),
                request.ProjectId
            ) ?? throw new NotFoundException("Project Member cannot be found.");

        _appTaskService.RemoveAppTask(task, projectMember);

        await _unitOfWork.Complete();

        await _eventBusSender.PublishMessage(new TaskDeletedEvent(task.Id));

        return;
    }
}
