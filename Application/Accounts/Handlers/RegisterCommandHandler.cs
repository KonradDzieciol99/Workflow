using Application.Accounts.Commands;
using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Core.Interfaces;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Handlers
{
    public class RegisterCommandHandler:IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly ICookiesService _cookiesService;
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IIdentityService _identityService;
        private readonly IRefreshTokenService _refreshTokenService;

        public RegisterCommandHandler(ICookiesService cookiesService,IMediator mediator, IJwtTokenService jwtTokenService, IIdentityService identityService,IRefreshTokenService refreshTokenService)
        {
            this._cookiesService = cookiesService;
            this._mediator = mediator;
            this._jwtTokenService = jwtTokenService;
            this._identityService = identityService;
            this._refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var user = await _identityService.CreateUserAsync(request.Email,request.Password);

            await _mediator.Publish(new UserCreatedEvent(user));

            return new AuthResponse() { Email = request.Email, UserDto = null, RedirectLink = "./auth/verifyEmail", IsEmailVerified = false };

            //var refreshToken = await _refreshTokenService.CreateRefreshToken(user);
            //_cookiesService.SetRefreshToken(refreshToken.ToString());
            //return new UserDto
            //{
            //    Email = user.Email,
            //    Token = await _jwtTokenService.CreateToken(user),
            //};
        }
    }
}
