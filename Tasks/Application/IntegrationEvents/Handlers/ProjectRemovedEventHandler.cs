using MediatR;
using MessageBus.Events;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents.Handlers
{
    public class ProjectRemovedEventHandler : IRequestHandler<ProjectRemovedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectRemovedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProjectRemovedEvent request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProjectMemberRepository.RemoveAllProjectMembersAsync(request.ProjectId);

            if (result > 0)
            {
                await Task.CompletedTask;
                return;
            }

            throw new InvalidOperationException("An error occurred while removing all project members.");

        }
    }
}
