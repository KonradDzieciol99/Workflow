using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Application.Messages.Commands;
using Chat.Application.Messages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Chat.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ApiScope")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SendMessageCommand command)
    {
         await _mediator.Send(command);

         return NoContent();
    }
    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> Get([FromQuery]string RecipientId, [FromQuery]string RecipientEmail, [FromQuery] int Skip, [FromQuery] int Take)
    {
        return await _mediator.Send(new GetMessageThreadQuery(RecipientEmail, RecipientId, Skip,Take));
    }


    ///// <summary>
    ///// Only available from the aggregate, as it is necessary to check unread messages
    ///// </summary>
    //[HttpGet]
    //public async Task<ActionResult<int>> GetUnreadMessagesCount()
    //{
    //    return await _mediator.Send(new GetUnreadMessagesCountQuery());
    //}
}
