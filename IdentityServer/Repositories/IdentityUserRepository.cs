using IdentityModel;
using IdentityServer.Common.Models;
using IdentityServer.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.Repositories
{
    public class IdentityUserRepository : Repository<IdentityUser>, IIdentityUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;

        public IdentityUserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            //this._userManager = userManager;
        }
        public async Task<List<SearchedUser>> FindUsersByEmailAsync(string seekerEmail, string emailOfSearchedUser)
        {
            //var usersStartingWithJohn = await _userManager.Users
            //    .Include(u => u.Claims)
            //    .Where(u => u.UserName.StartsWith(searchString))
            //    .ToListAsync();

            return await _applicationDbContext.Users
                .Where(user => user.Email.StartsWith(emailOfSearchedUser) && user.Email != seekerEmail)
                .Include(x => x.Claims)
                .Select(user => new SearchedUser
                {
                    Email = user.Email,
                    Id = user.Id,
                    PhotoUrl = user.Claims.Where(x=>x.ClaimType==JwtClaimTypes.Picture).Select(x=>x.ClaimValue).FirstOrDefault(),
                    //IsAlreadyInvited = user.FriendInvitationRecived.Concat(user.FriendInvitationSent).Any(x => x.InviterUser.Email == seekerEmail || x.InvitedUser.Email == seekerEmail),
                    //Confirmed = user.FriendInvitationSent.SingleOrDefault(h => h.InvitedUser.Email == seekerEmail).Confirmed || user.FriendInvitationRecived.SingleOrDefault(h => h.InviterUser.Email == seekerEmail).Confirmed,
                    //Confirmed = user.FriendInvitationRecived.Concat(user.FriendInvitationSent).SingleOrDefault(h => h.InvitedUserEmail == seekerEmail || h.InviterUserEmail == seekerEmail).Confirmed ? true : false
                })
                .ToListAsync();
            ///tutaj można wykorzystać deklaracje zmiennej i użyciej jej piozniej?
        }
    }
}
