using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Application.ProjectMembersAggregate.Commands;
using API.Aggregator.Application.ProjectsAggregate.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;

[Route("api/[controller]/{projectId}")]
[ApiController]
[Authorize(Policy = "ApiScope")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
    }

    [HttpPost("projectMembers/{email}")]
    public async Task<ActionResult<ProjectMemberDto?>> AddMember([FromRoute] string projectId, [FromRoute] string email, [FromQuery] ProjectMemberType type)
    {
        return await _mediator.Send(new AddProjectMemberAggregateCommand(email, type, projectId));
    }
    [HttpGet("projectMembers/SearchProjectMember/{email}")]
    public async Task<List<SearchedMemberDto>> searchMember([FromRoute] string email,
                                                            [FromQuery] string projectId,
                                                            [FromQuery] int take,
                                                            [FromQuery] int skip
                                                            )
    {
        return await _mediator.Send(new SearchProjectMemberAggregateQuery(email, projectId, take, skip));
    }
}