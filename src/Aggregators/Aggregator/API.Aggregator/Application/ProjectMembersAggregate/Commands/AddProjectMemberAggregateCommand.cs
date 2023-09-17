using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using API.Aggregator.Infrastructure.Services;
using MediatR;
using API.Aggregator.Application.Commons.Exceptions;

namespace API.Aggregator.Application.ProjectMembersAggregate.Commands;

public record AddProjectMemberAggregateCommand(string UserEmail,
                                               ProjectMemberType Type,
                                               string ProjectId) : IRequest<ProjectMemberDto>;

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberAggregateCommand, ProjectMemberDto>
{
    private readonly IProjectsService _projectsService;
    private readonly IIdentityServerService _identityServerService;

    public AddProjectMemberCommandHandler(IProjectsService projectsService, IIdentityServerService identityServerService)
    {
        _projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService));
        _identityServerService = identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
    }
    public async Task<ProjectMemberDto> Handle(AddProjectMemberAggregateCommand request, CancellationToken cancellationToken)
    {
        var usersFound = await _identityServerService.CheckIfUserExistsAsync(request.UserEmail) ?? throw new AggregatorDomainException("User cannot be found.", new NotFoundException());

        return await _projectsService.AddMember(request.ProjectId, new
        {
            UserId = usersFound.Id,
            UserEmail = usersFound.Email,
            usersFound.PhotoUrl,
            Type = ProjectMemberType.Member,
            request.ProjectId
        });
    }
}
