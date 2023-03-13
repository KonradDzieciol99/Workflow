using AutoMapper;
using Chat.Common.Models;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //[HttpGet("FindUsersByEmail/{email}")]//SEARCH
        //public async Task<ActionResult<IEnumerable<UserDto>>> FindUsersByEmailAsync(string email)
        //{


        //    return Ok(_mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(await _unitOfWork.UserRepository.FindUsersByEmailAsync(email, userEmail)));
        //}
        //[HttpGet("test/{email}")]
        //public async Task<ActionResult<IEnumerable<UserSearchedFriendInvitationDto>>> test(string email)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userEmail = User.FindFirstValue(ClaimTypes.Email);
        //    if (userEmail is null || userId is null)
        //    {return BadRequest("User cannot be identified"); }

        //    var friendIds = await _unitOfWork.FriendInvitationRepository.findAllFriendIds(userId);

        //    var users = await _unitOfWork.UserRepository.TEST(userEmail, email, friendIds);


        //    return Ok(users);
        //}
        //[HttpGet("test/{emailOfSearchedUser}")]
        //public async Task<ActionResult<IEnumerable<SearchedUserDto>>> test2(string emailOfSearchedUser)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userEmail = User.FindFirstValue(ClaimTypes.Email);
        //    if (userEmail is null || userId is null)
        //    { return BadRequest("User cannot be identified"); }
        //    var users = await _unitOfWork.UserRepository.FindUsersByEmailAsync(userEmail, emailOfSearchedUser);
        //    return Ok(users);
        //}

    }
}
