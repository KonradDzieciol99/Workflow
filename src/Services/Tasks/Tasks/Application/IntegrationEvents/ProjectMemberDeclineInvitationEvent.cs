﻿using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberDeclineInvitationEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string projectIconUrl) : IntegrationEvent;
public class ProjectMemberDeclineInvitationEventHandler : IRequestHandler<ProjectMemberDeclineInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberDeclineInvitationEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Handle(ProjectMemberDeclineInvitationEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteRemoveAsync(request.ProjectMemberId);

        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");
    }
}