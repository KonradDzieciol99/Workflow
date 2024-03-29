﻿using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityDuende.Domain.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityDuende.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userMgr;

    public ProfileService(UserManager<ApplicationUser> userMgr)
    {
        _userMgr = userMgr;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string sub = context.Subject.GetSubjectId();
        var user =
            await _userMgr.FindByIdAsync(sub)
            ?? throw new InvalidOperationException(
                $"User not found for the given subject identifier ({nameof(sub)})."
            );
        var claims = GetClaimsFromUser(user);

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string sub = context.Subject.GetSubjectId();
        var user = await _userMgr.FindByIdAsync(sub);
        context.IsActive = user != null;
    }

    private List<Claim> GetClaimsFromUser(ApplicationUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var claims = new List<Claim>
        {
            new(
                JwtClaimTypes.Email,
                user.Email ?? throw new InvalidOperationException($"{nameof(user.Email)} is null.")
            ),
        };

        if (user.PictureUrl is not null)
            claims.Add(new(JwtClaimTypes.Picture, user.PictureUrl));

        return claims;
    }
}
