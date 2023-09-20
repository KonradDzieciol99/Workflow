using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;
using Projects.Application.ProjectMembers.Commands;
using Projects.Application.ProjectMembers.Queries;
using Projects.Application.Projects.Commands;
using Projects.Application.Projects.Queries;

namespace Projects.Controllers;

[Authorize(Policy = "ApiScope")]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IMediator mediator;

    public ProjectsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpDelete("{projectId}/DeclineInvitation")]
    public async Task<IActionResult> DeclineProjectInvitation([FromRoute] string projectId)
    {
        await mediator.Send(new DeclineProjectInvitationCommand(projectId));

        return NoContent();
    }

    [Route("{projectId}/AcceptInvitation")]
    [HttpPut]
    public async Task<IActionResult> AcceptProjectInvitation([FromRoute] string projectId)
    {

        await mediator.Send(new AcceptProjectInvitationCommand(projectId));

        return NoContent();
    }

    [HttpPost("{projectId}/projectMembers/addMember")]
    public async Task<ActionResult<ProjectMemberDto?>> AddMember([FromRoute] string projectId, [FromBody] AddProjectMemberCommand command)
    {
        if (projectId != command.ProjectId)
            return BadRequest();

        return Ok(await mediator.Send(command));
    }

    [HttpDelete("{projectId}/projectMembers/{projectMemberId}")]
    public async Task<IActionResult> DeleteMember([FromRoute] string projectMemberId,
                                                  [FromRoute] string projectId)
    {
        await mediator.Send(new DeleteProjectMemberCommand(projectMemberId, projectId));

        return NoContent();
    }

    [HttpPut("{projectId}/projectMembers/{projectMemberId}")]
    public async Task<IActionResult> UpdateProjectMember([FromRoute] string projectMemberId,
                        [FromRoute] string projectId,
                        [FromBody] UpdateProjectMemberCommand command)
    {
        if (projectId != command.ProjectId && projectMemberId != command.UserId)
            return BadRequest();

        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{projectId}")]
    public async Task<ProjectDto> Get([FromRoute] string projectId)
    {
        return await mediator.Send(new GetProjectQuery(projectId));
    }

    [HttpGet]
    public async Task<ProjectsWithTotalCount> Get([FromQuery] GetProjectsQuery query)
    {
        return await mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectsWithTotalCount>> Create([FromBody] CreateProjectCommand command)
    {
        var result = await mediator.Send(command);

        var url = Request.Path + "/" + result.Id;

        return Created(url, result);
    }

    [HttpDelete("{projectId}")]
    public async Task<ActionResult<ProjectsWithTotalCount>> Delete([FromRoute] string projectId)
    {
        await mediator.Send(new DeleteProjectCommand(projectId));

        return NoContent();
    }

    [HttpPut("{projectId}")]
    public async Task<ActionResult<ProjectDto>> Put([FromRoute] string projectId, [FromBody] UpdateProjectCommand command)
    {
        if (projectId != command.ProjectId)
            return BadRequest();

        return await mediator.Send(command);
    }

    [HttpGet("{projectId}/GetMembersStatuses")]
    public async Task<ActionResult<List<MemberStatusDto>>> GetMembersStatuses([FromRoute] string projectId, [FromQuery] List<string> usersIds)
    {
        return await mediator.Send(new GetMembersStatusesQuery(projectId, usersIds));
    }

}
