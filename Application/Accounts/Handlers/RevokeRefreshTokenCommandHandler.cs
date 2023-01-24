using Application.Accounts.Commands;
using Application.Dtos;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowApi.Exceptions;

namespace Application.Accounts.Handlers
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICookiesService _cookiesService;

        public RevokeRefreshTokenCommandHandler(IRefreshTokenService refreshTokenService,ICookiesService cookiesService)
        {
            this._refreshTokenService = refreshTokenService;
            this._cookiesService = cookiesService;
        }

        public async Task<Unit> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = _cookiesService.GetRefreshToken();
            await _refreshTokenService.RevokeRefreshToken(refreshToken);
            _cookiesService.RemoveRefreshToken();
            return Unit.Value;
        }
    }
}
