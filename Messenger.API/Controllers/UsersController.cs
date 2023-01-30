using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socjal.API.Common.Models.Dto;
using Socjal.API.Entity;
using Socjal.API.Repositories;

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
        // GET: api/Invitations/5
        [HttpGet("FindUser/{email}")]
        public async Task<IEnumerable<UserDto>> FindUser(string email)
        {
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(await _unitOfWork.UserRepository.FindUsersByEmailAsync(email));
        }
    }
}
