using API.Aggregator.Application.Common.Models;
using API.Aggregator.Application.PhotosAggregate.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ApiScope")]
public class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PhotosController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<AppIcon> Post([FromForm] IFormFile file, [FromQuery] string projectId, [FromQuery] string name, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new ProjectIconUploadAggregateCommand(file, projectId, name), cancellationToken);
    }
}
