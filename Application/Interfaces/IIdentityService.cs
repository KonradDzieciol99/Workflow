using Application.Dtos;
using Domain.Identity.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Interfaces;

public interface IIdentityService
{
    //Task<string> GetUserNameAsync(string userId);

    //Task<bool> IsInRoleAsync(string userId, string role);

    //Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<AppUser> FindUserAsync(string email, string password);
    Task<AppUser> CreateUserAsync(string email,string password);
    Task<AppUser> SignInAsync(AppUser user,string password);
    Task<AppUser> FindRefreshTokenOwner(Guid refreshToken);
    //Task RevokeRefreshToken(Guid refreshToken,AppUser appUser);


    //Task<AppUser> DeleteUserAsync(string userId);
}
