using Application.Accounts.Commands;
using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Core.Interfaces;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICookiesService _cookiesService;

        public LoginCommandHandler(UserManager<AppUser> userManager,IIdentityService identityService,IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService,ICookiesService cookiesService)
        {
            this._userManager = userManager;
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._refreshTokenService = refreshTokenService;
            this._cookiesService = cookiesService;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindUserAsync(request.Email, request.Password);

            var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (isConfirmed is false)
            {
                return new AuthResponse() { Email = request.Email, UserDto = null, RedirectLink = "./auth/verifyEmail", IsEmailVerified = isConfirmed };
            }

            await _identityService.SignInAsync(user, request.Password);

            var refreshToken = await _refreshTokenService.CreateRefreshToken(user);
            _cookiesService.SetRefreshToken(refreshToken.ToString());

            var userDto = new UserDto
            {
                Email = user.Email,
                Token = await _jwtTokenService.CreateToken(user),
            };

            return new AuthResponse() {
            Email = request.Email,
            UserDto = userDto,
            IsEmailVerified = isConfirmed
            };

        }
    }
}
