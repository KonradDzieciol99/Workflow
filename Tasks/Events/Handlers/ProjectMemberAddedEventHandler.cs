using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;
using Tasks.Models;

namespace Tasks.Events.Handlers
{
    public class ProjectMemberAddedEventHandler : IRequestHandler<ProjectMemberAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectMemberAddedEventHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
        {

            var projectMember = new ProjectMember(request.ProjectMemberId,
                                                  request.UserId,
                                                  request.UserEmail,
                                                  request.PhotoUrl,
                                                  (ProjectMemberType)request.Type,
                                                  request.ProjectId);

            _unitOfWork.ProjectMemberRepository.Add(projectMember);

            if (!await _unitOfWork.Complete())
                throw new InvalidOperationException("An error occurred while adding a project member.");

            await Task.CompletedTask;
            return;      
        }
    }
}
