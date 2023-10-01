using API.Aggregator.Application.Common.Models;
using API.Aggregator.Application.FriendRequestsAggregate.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace API.Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FriendRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FriendRequestsController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("search/{email}")]
    public async Task<List<SearchedUserDto>> GetAsync(
        [FromRoute] string email,
        [FromQuery] int take,
        [FromQuery] int skip,
        CancellationToken cancellationToken
    )
    {
        return await _mediator.Send(new SearchFriendAggregateQuery(email, take, skip), cancellationToken);
    }
}
