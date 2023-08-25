using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Application.FriendRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ApiScope")]
public class FriendRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FriendRequestsController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Only available from the aggregate, as it is necessary to check whether the specified user exists
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FriendRequestDto>> Post([FromBody] CreateFriendRequestCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpGet("GetConfirmedFriendRequests")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetConfirmedFriendRequests([FromQuery] GetConfirmedFriendRequestsQuery query)
    {
        return await _mediator.Send(query);
    }

    [HttpGet("GetReceivedFriendRequests")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetReceivedFriendRequests()
    {
        return await _mediator.Send(new GetReceivedFriendRequestsQuery());
    }

    [HttpGet("GetFriendsStatus")]
    public async Task<ActionResult<List<FriendStatusDto>>> GetFriendsStatus([FromQuery] List<string> usersIds)
    {
        return await _mediator.Send(new GetFriendsStatusQuery(usersIds));
    }

    [HttpDelete("{TargetUserId}")]
    public async Task<IActionResult> Delete([FromRoute] string TargetUserId)
    {
        await _mediator.Send(new DeleteFriendRequestCommand(TargetUserId));
        
        return NoContent();
    }

    [HttpPut("{TargetUserId}")]
    public async Task<IActionResult> Put([FromRoute] string TargetUserId)
    {
        await _mediator.Send(new AcceptFriendRequestCommand(TargetUserId));

        return NoContent();
    }
}
