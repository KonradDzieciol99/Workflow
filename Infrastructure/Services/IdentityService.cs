using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkflowApi.Exceptions;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly SignInManager<AppUser> _signInManager;


    public IdentityService(UserManager<AppUser> userManager,IMapper mapper, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        this._mapper = mapper;
        this._signInManager = signInManager;
    }

    //public async Task<string> GetUserNameAsync(int userId)
    //{
    //    var user = await _userManager.Users.FirstAsync(u => u.Id == userId);
    //    return user.UserName;
    //}

    public async Task<AppUser> CreateUserAsync(string email, string password)
    {
        if (await _userManager.Users.AnyAsync(x => x.Email == email)) { throw new BadRequestException("Podany Email jest już zajęty."); }

        var user = new AppUser() { Email = email, UserName = email };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded) {throw new BadRequestException( String.Join(" ",result.Errors.Select(x => x.Description))); }

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded) { throw new BadRequestException(String.Join(" ", roleResult.Errors.Select(x => x.Description))); }

        return user;
    }

    public async Task<AppUser> FindRefreshTokenOwner(Guid refreshToken)
    {
        var user = await _userManager.Users.Where(x => x.RefreshTokens.Any(u => u.Token == refreshToken)).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new BadRequestException("user with such refresh Token does not exist");
        }

        return user;
    }

    //public async Task RevokeRefreshToken(Guid refreshToken, AppUser appUser)
    //{
    //    var refreshTokenFromDb = appUser.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
    //    if (refreshTokenFromDb == null)
    //    {
    //        throw new BadRequestException("RefreshToken does not exist");
    //    }
    //    refreshTokenFromDb.IsRevoked = true;

    //    var updateResoult = await _userManager.UpdateAsync(appUser);

    //    if (!updateResoult.Succeeded)
    //    {
    //        throw new BadRequestException(String.Join(" ", updateResoult.Errors.Select(x => x.Description)));
    //    }

    //    await Task.CompletedTask;
    //}
    public async Task<AppUser> FindUserAsync(string email, string password)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);

        if (user == null) { throw new BadRequestException("Błędne Dane Logowania"); }
        return user;
    }
    public async Task<AppUser> SignInAsync(AppUser user, string password)
    {

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded) { throw new BadRequestException("Login failed"); };

        return user;
    }

    //public async Task<bool> IsInRoleAsync(int userId, string role)
    //{
    //    var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

    //    return user != null && await _userManager.IsInRoleAsync(user, role);
    //}

    //public async Task<bool> AuthorizeAsync(int userId, string policyName)
    //{
    //    var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

    //    if (user == null)
    //    {
    //        return false;
    //    }

    //    //var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

    //    //var result = await _authorizationService.AuthorizeAsync(principal, policyName);

    //    return result.Succeeded;
    //}

    //public async Task<Result> DeleteUserAsync(string userId)
    //{
    //    var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

    //    return user != null ? await DeleteUserAsync(user) : Result.Success();
    //}

    //public async Task<Result> DeleteUserAsync(AppUser user)
    //{
    //    var result = await _userManager.DeleteAsync(user);

    //    return result.ToApplicationResult();
    //}
}
