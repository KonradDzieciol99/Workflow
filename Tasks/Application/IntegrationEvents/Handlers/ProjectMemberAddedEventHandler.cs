using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents.Handlers
{
    public class ProjectMemberAddedEventHandler : IRequestHandler<ProjectMemberAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectMemberAddedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
        {

            var projectMember = new ProjectMember(request.ProjectMemberId,
                                                  request.UserId,
                                                  request.UserEmail,
                                                  request.PhotoUrl,
                                                  (ProjectMemberType)request.Type,
                                                  (InvitationStatus)request.InvitationStatus,
                                                  request.ProjectId);

            _unitOfWork.ProjectMemberRepository.Add(projectMember);

            if (!await _unitOfWork.Complete())
                throw new InvalidOperationException("An error occurred while adding a project member.");

            await Task.CompletedTask;
            return;
        }
    }
}
