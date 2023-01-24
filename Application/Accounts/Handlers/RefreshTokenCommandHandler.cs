using Application.Accounts.Commands;
using Application.Dtos;
using Application.Interfaces;
using Core.Interfaces;
using Core.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Identity.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Handlers
{
    internal class RefreshTokenCommandHandler:IRequestHandler<RefreshTokenCommand, UserDto>
    {
        private readonly ICookiesService _cookiesService;
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(ICookiesService cookiesService,IIdentityService identityService, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService,IUnitOfWork unitOfWork)
        {
            this._cookiesService = cookiesService;
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._refreshTokenService = refreshTokenService;
            this._unitOfWork = unitOfWork;
        }

        public async Task<UserDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var endOfLifeRefreshToken = _cookiesService.GetRefreshToken();
            //var refreshToken = await _unitOfWork.Repository<RefreshToken>().ListAllAsync();

            var user = await _identityService.FindRefreshTokenOwner(endOfLifeRefreshToken);
            await _refreshTokenService.RevokeRefreshToken(endOfLifeRefreshToken);

            var newRefreshToken = await _refreshTokenService.CreateRefreshToken(user);
            var jwtToken = await _jwtTokenService.CreateToken(user);
            _cookiesService.SetRefreshToken(newRefreshToken.ToString());

            return new UserDto
            {
                Email = user.Email,
                Token = await _jwtTokenService.CreateToken(user),
            };
        }
    }
}
