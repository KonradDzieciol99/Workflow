using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.AppTasks.Queries;
using Tasks.Application.Common.Models;

namespace Tasks.Controllers;

[Authorize(Policy = "ApiScope")]
[Route("api/projects/{projectId}/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        this._mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult<AppTaskDtosWithTotalCount>> Get([FromRoute] string projectId, [FromQuery] GetAppTasksQuery query)
    {
        if (projectId != query.ProjectId)
            return BadRequest();


        var result = await _mediator.Send(query);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppTaskDto>> Get([FromRoute] string projectId, [FromRoute] string id)
    {
        return await _mediator.Send(new GetAppTaskQuery(id, projectId));
    }

    [HttpPost]
    public async Task<ActionResult<AppTaskDto>> Post([FromRoute] string projectId, [FromBody] AddTaskCommand command)
    {

        if (projectId != command.ProjectId)
            return BadRequest();

        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(Get), new { id = result.Id, projectId = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppTaskDto>> Put([FromRoute] string projectId, [FromRoute] string id, [FromBody] UpdateAppTaskCommand command)
    {
        if (projectId != command.ProjectId || id != command.Id)
            return BadRequest();

        return await _mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] string projectId, [FromRoute] string id)
    {

        await _mediator.Send(new DeleteAppTaskCommand(id, projectId));

        return NoContent();
    }
}
