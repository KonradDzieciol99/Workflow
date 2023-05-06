using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Entity;
using Tasks.Repositories;

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

            var projectMember = new ProjectMember()
            {
                Id = request.ProjectMemberId,
                UserEmail = request.UserEmail,
                UserId = request.UserId,
                Type = (ProjectMemberType)request.Type,
                PhotoUrl = request.PhotoUrl,
                ProjectId = request.ProjectId
            };

            _unitOfWork.ProjectMemberRepository.Add(projectMember);

            if (!await _unitOfWork.Complete())
                throw new InvalidOperationException("An error occurred while adding a project member.");

            await Task.CompletedTask;
            return;      
        }
    }
}
