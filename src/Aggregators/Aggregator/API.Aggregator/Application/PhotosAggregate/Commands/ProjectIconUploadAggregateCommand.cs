using API.Aggregator.Application.Common.Models;
using API.Aggregator.Infrastructure.Services;
using HttpMessage.Exceptions;
using HttpMessage.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Application.PhotosAggregate.Commands;

public record ProjectIconUploadAggregateCommand(IFormFile File, string ProjectId,  string Name) : IRequest<AppIcon>;

public class ProjectIconUploadAggregateCommandHandler
    : IRequestHandler<ProjectIconUploadAggregateCommand, AppIcon>
{
    private readonly IProjectsService _projectsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPhotosService _photosService;

    public ProjectIconUploadAggregateCommandHandler(
        IProjectsService projectsService,
        ICurrentUserService currentUserService,
        IPhotosService photosService
    )
    {
        _projectsService =
            projectsService ?? throw new ArgumentNullException(nameof(projectsService));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._photosService = photosService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<AppIcon> Handle(
        ProjectIconUploadAggregateCommand request,
        CancellationToken cancellationToken
    )
    {
        var isMember =
            await _projectsService.CheckIfUserIsAMemberOfProject(_currentUserService.GetUserId(), request.ProjectId,cancellationToken);

        if (!isMember)
            throw new ForbiddenException("User is not a member of this project.");


        return await _photosService.AddProjectAppIcon(request.File, request.ProjectId, request.Name, cancellationToken);
    }
}
