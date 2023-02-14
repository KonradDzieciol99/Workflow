using AutoMapper;
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

        [HttpGet("FindUsersByEmail/{email}")]//SEARCH
        public async Task<IEnumerable<UserDto>> FindUsersByEmailAsync(string email)
        {
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(await _unitOfWork.UserRepository.FindUsersByEmailAsync(email));
        }
        [HttpGet("test/{email}")]
        public async Task<IEnumerable<UserSearchedFriendInvitationDto>> test(string email)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var friendIds = await _unitOfWork.FriendInvitationRepository.findAllFriendIds(userId);

            var users = await _unitOfWork.UserRepository.TEST(userEmail, email, friendIds);


            return users;
        }

    }
}
