using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Application.FriendRequestsAggregate.Queries;
using API.Aggregator.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [FromQuery] int skip
    )
    {
        return await _mediator.Send(new SearchFriendAggregateQuery(email, take, skip));
    }
}
