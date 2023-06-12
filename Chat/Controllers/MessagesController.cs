using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Application.Messages.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<List<MessageDto>>> Get([FromBody] CreateFriendRequestCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
