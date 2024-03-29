﻿using Chat.Application.Common.Models;
using Chat.Application.Messages.Commands;
using Chat.Application.Messages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        return Created("", null);
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> Get([FromQuery] GetMessageThreadQuery query)
    {
        return await _mediator.Send(query);
    }
}
