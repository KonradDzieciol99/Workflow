using Domain.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRefreshTokenService
    {
        //void SetRefreshTokenInCookie(Guid guid);
        Task<Guid> CreateRefreshToken(AppUser appUser);
        //Guid GetRefreshTokenFromCookie();
        Task RevokeRefreshToken(Guid refreshToken);
    }
}
