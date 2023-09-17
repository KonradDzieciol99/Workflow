using API.Aggregator.Application.Commons.Exceptions;
using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using API.Aggregator.Infrastructure.Services;
using API.Aggregator.Services;
using MediatR;

namespace API.Aggregator.Application.AppTasksAggregate.Commands;
public record CreateAppTaskAggregateCommand(string Name,
                                            string? Description,
                                            string ProjectId,
                                            string? TaskAssigneeMemberId,
                                            Priority Priority,
                                            State State,
                                            DateTime DueDate,
                                            DateTime StartDate,
                                            string? TaskLeaderId) : IRequest<AppTaskDto>;
public class CreateAppTaskAggregateCommandHandler : IRequestHandler<CreateAppTaskAggregateCommand, AppTaskDto>
{
    private readonly ITaskService _taskService;
    private readonly IProjectsService _projectsService;
    private readonly ICurrentUserService _currentUserService;

    public CreateAppTaskAggregateCommandHandler(ITaskService taskService, IProjectsService projectsService,ICurrentUserService currentUserService)
    {
        this._taskService = taskService ?? throw new ArgumentNullException(nameof(taskService)); ;
        this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<AppTaskDto> Handle(CreateAppTaskAggregateCommand request, CancellationToken cancellationToken)
    {
        var projectsServiceResult = await _projectsService.CheckIfUserIsAMemberOfProject(_currentUserService.GetUserId(), request.ProjectId);

        if (!projectsServiceResult)
            throw new AggregatorDomainException("You are not a member of this project", new ForbiddenAccessException());

        return await _taskService.CreateTask(request);
    }
}