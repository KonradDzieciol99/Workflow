using API.Aggregator.Application.AppTasksAggregate.Commands;
using API.Aggregator.Application.Commons.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "ApiScope")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
    }

    [HttpPost]
    public async Task<ActionResult<AppTaskDto>> Post([FromBody] CreateAppTaskAggregateCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("", result);
    }
}
