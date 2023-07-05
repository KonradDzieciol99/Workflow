using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.AppNotifications.Commands;
using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Entity;

namespace Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiScope")]
    public class AppNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppNotificationController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<List<AppNotification>>> Get([FromQuery] GetAppNotificationsQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> MarkAsSeen([FromRoute] string id)
        {
            await _mediator.Send(new DeleteAppNotificationCommand(id));

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            await _mediator.Send(new DeleteAppNotificationCommand(id));

            return NoContent();
        }
    }
}
