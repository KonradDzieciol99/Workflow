using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Photos.Application.Icon.Commands;
using Photos.Application.Icon.Queries;
using Photos.Domain.Entity;

namespace Photos.Controllers;

[Authorize(Policy = "ApiScope")]
[Route("api/[controller]")]
[ApiController]
public class IconController : ControllerBase
{
    private readonly IMediator _mediator;

    public IconController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Only accessible from the inside.
    /// </summary>

    [HttpPost]
    public async Task<AppIcon> Post([FromForm] IFormFile file, [FromQuery] string projectId, [FromQuery] string name)
    {
        return await _mediator.Send(new ProjectIconUploadCommand(file, projectId, name));
    }

    [HttpGet]
    public async Task<ActionResult<List<AppIcon>>> Get([FromQuery] string? projectId)
    {
        return await _mediator.Send(new GetProjectsIconsQuery(projectId));
    }
}
