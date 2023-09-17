using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Infrastructure.Services;
using API.Aggregator.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Application.ProjectsAggregate.Queries;

public record SearchProjectMemberAggregateQuery(string Email, string ProjectId, int Take, int Skip) : IRequest<List<SearchedMemberDto>>
{
}
public class SearchMemberQueryHandler : IRequestHandler<SearchProjectMemberAggregateQuery, List<SearchedMemberDto>>
{
    private readonly IProjectsService _projectsService;
    private readonly IIdentityServerService _identityServerService;

    public SearchMemberQueryHandler(IProjectsService projectsService, IIdentityServerService identityServerService)
    {
        _projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService));
        _identityServerService = identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
    }

    public async Task<List<SearchedMemberDto>> Handle(SearchProjectMemberAggregateQuery request, CancellationToken cancellationToken)
    {
        var usersFound = await _identityServerService.SearchAsync(request.Email, request.Take, request.Skip);

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedMemberDto>();

        var status = await _projectsService.GetMembersStatuses(usersFound.Select(x => x.Id).ToList(), request.ProjectId);

        var SearchedMembers = usersFound.Select((x, index) => new SearchedMemberDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)).ToList();

        return SearchedMembers;
    }
}

