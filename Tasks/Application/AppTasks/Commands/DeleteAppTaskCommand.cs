using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Authorization.Requirements;
using Tasks.Infrastructure.Repositories;
using Tasks.Services;
using Tasks.Application.Common.Exceptions;
using Tasks.Domain.Exceptions;

namespace Tasks.Application.AppTasks.Commands;

public record DeleteAppTaskCommand(string Id,string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectManagmentOrTaskAuthorRequirement(ProjectId,Id)
        };
        return listOfRequirements;
    }
}
public class DeleteAppTaskCommandHandler : IRequestHandler<DeleteAppTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public DeleteAppTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IAzureServiceBusSender messageBus)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        this._azureServiceBusSender = messageBus ?? throw new ArgumentNullException(nameof(_azureServiceBusSender)); ;
    }
    public async Task Handle(DeleteAppTaskCommand request, CancellationToken cancellationToken)
    {

        var task = await _unitOfWork.AppTaskRepository.GetAsync(request.Id) ?? throw new TaskDomainException(string.Empty, new BadRequestException("Task cannot be found.")) ;
        
        _unitOfWork.AppTaskRepository.Remove(task);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        await _azureServiceBusSender.PublishMessage(new TaskDeletedEvent(task.Id));

        return;

    }
}