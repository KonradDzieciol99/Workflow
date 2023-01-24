using Application.Interfaces;
using Core.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowApi.Exceptions;

namespace Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ICookiesService _cookiesService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenService(ICookiesService cookiesService, IUnitOfWork unitOfWork)
        {
            this._cookiesService = cookiesService;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateRefreshToken(AppUser appUser)
        {
            var refreshToken = new RefreshToken()
            {
                Token = Guid.NewGuid(),
                UserId = appUser.Id
            };

            _unitOfWork.RefreshTokensRepository.Add(refreshToken);

            if (await _unitOfWork.Complete())
            {
                return refreshToken.Token;
            }

            throw new Exception("error on CreateRefreshToken");

        }

        //public Guid GetRefreshTokenFromCookie()
        //{
        //    string? refreshTokenAsString = _cookiesService.GetRefreshToken();

        //    if (string.IsNullOrEmpty(refreshTokenAsString))
        //    {
        //        throw new BadRequestException("Empty cookie");
        //    }

        //    if (Guid.TryParse(refreshTokenAsString, out Guid refreshTokenAsGuid))
        //    {
        //        return refreshTokenAsGuid;
        //    }

        //    throw new BadRequestException("Bad format of cookie");
        //}
        //public void SetRefreshTokenInCookie(Guid refreshToken)
        //{
        //    _cookiesService.SetRefreshToken(refreshToken.ToString());
        //}

        public async Task RevokeRefreshToken(Guid refreshToken)
        {
            var refreshTokenFromDb  = await _unitOfWork.RefreshTokensRepository.FindOneAsync(x => x.Token == refreshToken);
            if (refreshTokenFromDb == null)
            {
                throw new BadRequestException("RefreshToken does not exist");
            }
            refreshTokenFromDb.IsRevoked = true;

            if (await _unitOfWork.Complete())
            {
                await Task.CompletedTask;
                return;
            }

            throw new BadRequestException("Can't revoke RefreshToken");
        }
    }
}
