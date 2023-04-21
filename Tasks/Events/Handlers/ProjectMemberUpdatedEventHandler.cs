using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Repositories;

namespace Tasks.Events.Handlers
{
    public class ProjectMemberUpdatedEventHandler : IRequestHandler<ProjectMemberUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectMemberUpdatedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        public async Task Handle(ProjectMemberUpdatedEvent request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProjectMemberRepository.UpdateAsync(request.ProjectId, request.UserId, (ProjectMemberType)request.Type);

            if (!await _unitOfWork.Complete())
                throw new InvalidOperationException("An error occurred while updating a project member.");

            await Task.CompletedTask;
            return;
        }
    }
}
