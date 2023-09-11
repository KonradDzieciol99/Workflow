using IdentityDuende.Application.Models;
using IdentityDuende.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityDuende.Controlers;

[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class IdentityUserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserController(UserManager<ApplicationUser> userManager)
    {
        this._userManager = userManager;
    }
    [HttpGet("search/{email}")]
    public async Task<ActionResult<List<UserDto>>> Search([FromRoute] string email,[FromQuery] int take, [FromQuery]int skip)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (userEmail is null || userId is null)
         return BadRequest("User cannot be identified"); 

        var users = await _userManager.Users
                    .Where(user => user.Email.StartsWith(email) && user.Email != userEmail)
                    .Skip(skip)
                    .Take(take)
                    .Select(x => new UserDto(x.Id, x.Email, x.PictureUrl))
                    .ToListAsync();

        return Ok(users);
    }

    [HttpGet("CheckIfUserExists/{email}")]
    public async Task<ActionResult<UserDto?>> CheckIfUserExists(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Ok(null);

        return Ok(new UserDto(user.Id, user.Email!, user.PictureUrl));
    }
}
