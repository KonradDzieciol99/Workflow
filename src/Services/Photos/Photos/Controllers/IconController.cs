using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photos.Application.Icon.Commands;
using Photos.Application.Icon.Queries;
using Photos.Common.Models;

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

    //TODO
    //[HttpPost]
    //public async Task<IActionResult> Post([FromForm] IFormFile file, [FromQuery] string name)
    //{
    //    await _mediator.Send(new IconUploadCommand(file,name));
    //    return Ok();
    //}

    [HttpGet]
    public async Task<ActionResult<List<Icon>>> Get()
    {
        return Ok(await _mediator.Send(new GetProjectsIconsQuery()));
    }
}
