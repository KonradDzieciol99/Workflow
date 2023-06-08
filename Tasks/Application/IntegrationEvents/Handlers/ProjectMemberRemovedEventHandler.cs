using AutoMapper;
using MediatR;
using MessageBus.Events;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Events.Handlers
{
    public class ProjectMemberRemovedEventHandler : IRequestHandler<ProjectMemberRemovedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectMemberRemovedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        public async Task Handle(ProjectMemberRemovedEvent request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProjectMemberRepository.RemoveAsync(request.ProjectMemberId);

            if(result > 0)
            {
                await Task.CompletedTask;
                return;
            }

            throw new InvalidOperationException("An error occurred while removing a project member.");
        }
    }
}
