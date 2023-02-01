using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Socjal.API.Common.Models;
using Socjal.API.Dto;
using Socjal.API.Entity;
using Socjal.API.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Socjal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendInvitationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FriendInvitationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        // GET: api/Invitations/5
        [HttpPost]
        public async Task<IActionResult> InviteFriend(UserDto InitedUserDto)
        {
            var InviterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");
            var Inviter = await _unitOfWork.UserRepository.GetUserByIdAsync(InviterId);
            var InitedUser = await _unitOfWork.UserRepository.GetUserByIdAsync(InitedUserDto.Id);

            if (Inviter is null || InitedUser is null)
            {
                return BadRequest("The specified users do not exist.");
            }
            if (_unitOfWork.FriendInvitationRepository.checkIfExistsAsync(InviterId, InitedUserDto.Id) is not null)
            {
                return BadRequest("This invitation is already exist.");
            }

            FriendInvitation friendInvitation = new ()
            {
                InviterUser = Inviter,
                InvitedUser = InitedUser,
                InviterUserId = InviterId,
                InvitedUserId = InitedUserDto.Id,
            };

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("User cannot be invited.");

        }

        //[HttpGet("FindUsersByEmailAndCheckState/{email}")]//SEARCH
        //public async Task<ActionResult<IEnumerable<UserSearchedFriendInvitationDto>>> FindUsersByEmailAndCheckState(string email)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var users = await _unitOfWork.UserRepository.FindUsersByEmailAsync(email);

        //    var tasks = new List<Task>();

        //    foreach (var item in users)
        //    {
        //        var task = _unitOfWork.FriendInvitationRepository.checkIfExistsAsync(item.Id,userId);
        //        tasks.Add(task);
        //    }

        //    await Task.WhenAll(tasks);

        //    var userSearcheds = new List<UserSearchedFriendInvitationDto>();
        //    for (int i = 0; i < tasks.Count; i++)
        //    {
        //        var user = users.ElementAt(i) ?? throw new Exception("can't find user");

        //        var finishedTask = ((Task<FreindInvitationRelationStatus>)tasks.ElementAt(i)).Result;

        //        var UserSearched = new UserSearchedFriendInvitationDto()
        //        {
        //            Id = user.Id,
        //            Email = user.Email,
        //            PhotoUrl = user.PhotoUrl,
        //            IsAlreadyInvited = finishedTask.IsAlreadyInvited,
        //            Confirmed = finishedTask.Confirmed
        //        };

        //        userSearcheds.Add(UserSearched);
        //    }

        //    return userSearcheds;
        //    //return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(await _unitOfWork.UserRepository.FindUsersByEmailAsync(email));
        //}
        //[HttpPost]
        //public async Task<IActionResult> PostFriendInvitation(string friendInvitationDto)
        //{

        //    var FriendInvitation = new FriendInvitation()
        //    {
        //        Confirmed = false,
        //        InviterUser =,
        //        InviterUserId,
        //        InviterUserEmail,
        //        InvitedUser,
        //        InvitedUserId,
        //        InvitedUserEmail,
        //    };

        //    await _unitOfWork.UserRepository.

        //    return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(await _unitOfWork.UserRepository.FindUsersByEmailAsync(email));
        //}
    }
}
