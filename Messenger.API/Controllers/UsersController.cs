using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socjal.API.Dto;
using Socjal.API.Entity;
using Socjal.API.Repositories;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Socjal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
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
