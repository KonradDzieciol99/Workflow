using Application.Accounts.Commands;
using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WorkflowApi.Controllers
{
    //[AutoValidateAntiforgeryToken]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            return await _mediator.Send(new RefreshTokenCommand());
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            await _mediator.Send(new RevokeRefreshTokenCommand());
            return NoContent();
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            //await _mediator.Send(new ResetPasswordCommand());
            return NoContent();
        }
        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromQuery]string email)
        { 
            var s = email;
            //await _mediator.Send(new ResendVerificationEmailCommand() { });
            return NoContent();
        }
        [HttpPost("test")]
        public async Task<IActionResult> test()
        {
            return Redirect("~/auth/login");

            //return RedirectToAction("Index");
            //return Redirect("htp/register");
            
        }
    }
}
