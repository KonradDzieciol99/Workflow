using Chat.Dto;
using Chat.Entity;
using Chat.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await applicationDbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
        public async Task<User?> GetUserByIdAsync(string Id)
        {
            return await applicationDbContext.Users.SingleOrDefaultAsync(x => x.Id == Id);
        }
        public async Task<IEnumerable<User>> FindUsersByEmailAsync(string email)
        {
            return await applicationDbContext.Users.Where(user => user.Email.StartsWith(email)).ToListAsync();
        }
        public async Task<IEnumerable<UserSearchedFriendInvitationDto>> TEST(string userEmail, string searchedUseremail, IEnumerable<string> IdsInRelation)
        {
            return await applicationDbContext.Users
                .Include(x => x.FriendInvitationSent)
                .Include(x => x.FriendInvitationRecived)
                .Where(user => user.Email.StartsWith(searchedUseremail))
                .Select(x => new UserSearchedFriendInvitationDto
                {
                    Email = x.Email,
                    Id = x.Id,
                    PhotoUrl = x.PhotoUrl,
                    IsAlreadyInvited = IdsInRelation.Contains(x.Id),
                    Confirmed = IdsInRelation.Contains(x.Id) && (x.FriendInvitationSent.SingleOrDefault(h => h.InvitedUser.Email == userEmail).Confirmed || x.FriendInvitationRecived.SingleOrDefault(h => h.InviterUser.Email == userEmail).Confirmed)
                    //?  :
                    // ? true : false
                    //x.FriendInvitationSent.Select(b => b.InvitedUser.Email).Contains(userEmail) ? true :

                    //IsAlreadyInvited = x.FriendInvitationSent.Select(b => b.InviterUser.Email).Contains(userEmail) ? true :
                    //                   x.FriendInvitationSent.Select(b => b.InvitedUser.Email).Contains(userEmail) ? true : false,
                    //Confirmed = searchedUseremail.Contains(userEmail) && x.,
                })
                .ToListAsync();
        }



    }
}
