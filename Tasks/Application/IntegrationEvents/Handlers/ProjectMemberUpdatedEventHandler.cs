using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents.Handlers;

public class ProjectMemberUpdatedEventHandler : IRequestHandler<ProjectMemberUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberUpdatedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Handle(ProjectMemberUpdatedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteUpdateAsync(request.ProjectMemberId,
                                                                           (ProjectMemberType)request.Type
                                                                           , (InvitationStatus)request.InvitationStatus);

        //if (!await _unitOfWork.Complete())
        //    throw new InvalidOperationException("An error occurred while updating a project member.");
        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");

        //await Task.CompletedTask;
        //return;
    }
}
