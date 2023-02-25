using AutoMapper;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendInvitationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureServiceBusSender _messageBus;

        public FriendInvitationController(IUnitOfWork unitOfWork, IMapper mapper, IAzureServiceBusSender messageBus)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._messageBus = messageBus;
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
            if (await _unitOfWork.FriendInvitationRepository.checkIfExistsAsync(InviterId, InitedUserDto.Id))
            {
                return BadRequest("This invitation is already exist.");
            }
            if (Inviter.Id == InitedUser.Id)
            {
                return BadRequest("You can't invite yourself.");
            }
            FriendInvitation friendInvitation = new()
            {
                InviterUserId = Inviter.Id,
                InviterUserEmail = Inviter.Email,
                InviterPhotoUrl = Inviter.PhotoUrl,
                InviterUser = Inviter,
                InvitedUserEmail = InitedUser.Email,
                InvitedUserId = InitedUser.Id,
                InvitedPhotoUrl = InitedUser.PhotoUrl,
                InvitedUser = InitedUser,
                Confirmed = false
            };

            _unitOfWork.FriendInvitationRepository.Add(friendInvitation);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("User cannot be invited.");

        }
        [HttpGet("GetAllFriends")]//with presence
        public async Task<ActionResult<UserDto>> GetAllFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null || userId is null)
            {
                return BadRequest("User cannot be identified.");
            }


            var friendsInvitation = await _unitOfWork.FriendInvitationRepository.GetAllFriends(userId);

            if (friendsInvitation == null)
            {
                return NotFound();
            }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);
            //
            var users = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            var newOnlineMessagesUserWithFriendsEvent = new NewOnlineMessagesUserWithFriendsEvent() { NewOnlineUserChatFriends = users, NewOnlineUser = new SimpleUser() { UserEmail = userEmail, UserId = userId } };
            await _messageBus.PublishMessage(newOnlineMessagesUserWithFriendsEvent, "new-online-messages-user-with-friends-queue");
            //
            return Ok(friendsInvitationDtos);
        }
        [HttpGet("GetAllInvitations")]
        public async Task<ActionResult<IEnumerable<FriendInvitationDto>>> GetAllInvitations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

            var friendsInvitation = await _unitOfWork.FriendInvitationRepository.GetAllInvitations(userId);

            if (friendsInvitation == null)
            {
                return BadRequest();
            }

            var friendsInvitationDto = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);

            return Ok(friendsInvitationDto);
        }
        [HttpPost("AcceptFriendInvitation")]
        public async Task<IActionResult> AcceptFriendInvitation(FriendInvitationDto friendInvitationDto)
        {
            var InvitedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

            var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, InvitedUserId);

            if (friendInvitation == null)
                return BadRequest("invitation does not exist");

            friendInvitation.Confirmed = true;

            if (await _unitOfWork.Complete())
            {
                //
                //var users = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
                var friendInvitationAcceptedEvent = new FriendInvitationAcceptedEvent() { FriendInvitationDto = _mapper.Map<FriendInvitationDtoGlobal>(friendInvitation) };
                await _messageBus.PublishMessage(friendInvitationAcceptedEvent, "friend-invitation-accepted-queue");
                //
                return Ok();
            };

            return BadRequest("The invitation cannot be confirmed.");
        }
        [HttpPost("DeclineFriendInvitation")]
        public async Task<IActionResult> DeclineFriendInvitation(FriendInvitationDto friendInvitationDto)
        {
            var InitedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

            //var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, InitedUserId);

            var friendInvitation = _mapper.Map<FriendInvitationDto, FriendInvitation>(friendInvitationDto);

            if (friendInvitation == null)
                return BadRequest("invitation does not exist");

            _unitOfWork.FriendInvitationRepository.Remove(friendInvitation);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("This invitation cannot be canceled.");
        }

    }
}
