using Duende.IdentityServer;
using IdentityDuende.Entities;
using IdentityDuende.Infrastructure.Repositories;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityDuende.Controlers
{
    [Route("api/[controller]")]
    //[Authorize]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    [ApiController]
    public class IdentityUserController : ControllerBase
    {
        //private readonly IChatServiceHttpRequest _chatServiceHttpRequest;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUserController(/*IChatServiceHttpRequest chatServiceHttpRequest,*/ IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            //_chatServiceHttpRequest = chatServiceHttpRequest;
            _unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        //[HttpGet("search/{emailOfSearchedUser}")]
        //public async Task<ActionResult<IEnumerable<UserFriendStatusToTheUser>>> test22(string emailOfSearchedUser)
        //{
        //    var userId = User.FindFirstValue(JwtClaimTypes.Subject); //typy claimow są inne bo nie pobieramy ich przez 
        //                                                             //standardowy AddJwtBearer od microsoft tylko przez 
        //                                                             // jakiś rodzaj od IdentytyServer AddLocalApiAuthentication
        //    var userEmail = User.FindFirstValue(JwtClaimTypes.Email);

        //    foreach (var claim in User.Claims)
        //    {
        //        Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
        //    }

        //    if (userEmail is null || userId is null)
        //    { return BadRequest("User cannot be identified"); }

        //    var users = await _unitOfWork.IdentityUserRepository.FindUsersByEmailAsync(userEmail, emailOfSearchedUser);

        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    //[Authorization, { Bearer 
        //    var token = HttpContext.Request.Headers["Authorization"][0].Remove(0, "Bearer ".Length);

        //    //var test = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization").Value;
        //    //var accessTokensss = User.FindFirstValue("access_token");
        //    var userFriendStatus = await _chatServiceHttpRequest.GetFriendsStatus(userId, users.Select(x => x.Id), token);

        //    //var resoulttest = users.SelectMany(c => userFriendStatus);

        //    //var resoult = users.GroupJoin(userFriendStatus, x => x.Id, c => c.UserId, (x, c) => 
        //    //    new SearchedUser() 
        //    //    {
        //    //        Id=x.Id,
        //    //        Email=x.Email,
        //    //        PhotoUrl=x.PhotoUrl,
        //    //        Status = c.Where(v=>v.UserId == userId).FirstOrDefault()?.Status ?? UserFriendStatusType.Stranger,
        //    //    }
        //    //);
        //    users.ForEach(user => user.Status = userFriendStatus.FirstOrDefault(v => v.UserId == user.Id)?.Status ?? UserFriendStatusType.Stranger);


        //    //foreach (var user in users)
        //    //{
        //    //    user.Status = userFriendStatus.FirstOrDefault(v => v.UserId == userId)?.Status ?? UserFriendStatusType.Stranger;
        //    //}


        //    //var usersStatus = await _unitOfWork.FriendInvitationRepository.GetFriendsStatusAsync(searcherId, ids);

        //    return Ok(users);
        //}
        [HttpGet("CheckIfUserExists/{email}")] //m2m
        public async Task<ActionResult<bool?>> CheckIfUserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user is null)
                return Ok(false);
            
            return Ok(true);
        }
    }
}
