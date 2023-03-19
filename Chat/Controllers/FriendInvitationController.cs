using AutoMapper;
using Chat.Common.Models;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Chat.Services;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Amqp.Framing;
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
        private readonly IIdentityServerService _identityServerService;

        public FriendInvitationController(IUnitOfWork unitOfWork, IMapper mapper, IAzureServiceBusSender messageBus, IIdentityServerService identityServerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._messageBus = messageBus;
            this._identityServerService = identityServerService;
        }
        // GET: api/Invitations/5
        [HttpPost]
        public async Task<IActionResult> InviteFriend(UserDto InitedUserDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userPhotUrl = User.FindFirst("picture")?.Value;
            //var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null || userId is null)
            {
                return BadRequest("User cannot be identified.");
            }

            var token = await HttpContext.GetTokenAsync("access_token");//"Bearer", 
            var InitedUser = await _identityServerService.CheckIfUserExistsAsync<UserDto?>(InitedUserDto.Email, token);

            if (InitedUser is null)
            {
                return BadRequest("The specified users do not exist.");
            }

            //var Inviter = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            //var InitedUser = await _unitOfWork.UserRepository.GetUserByIdAsync(InitedUserDto.Id);

            //if (Inviter is null || InitedUser is null)
            //{
            //    return BadRequest("The specified users do not exist.");
            //}
            if (await _unitOfWork.FriendInvitationRepository.checkIfExistsAsync(userId, InitedUserDto.Id))
            {
                return BadRequest("This invitation is already exist.");
            }
            if (userEmail == InitedUserDto.Email)
            {
                return BadRequest("You can't invite yourself.");
            }

            FriendInvitation friendInvitation = new()
            {
                InviterUserId = userId,
                InviterUserEmail = userEmail,
                InviterPhotoUrl = userPhotUrl,
                //InviterUser = Inviter,
                InvitedUserEmail = InitedUser.Email,
                InvitedUserId = InitedUser.Id,
                InvitedPhotoUrl = InitedUser.PhotoUrl,
                //InvitedUser = InitedUser,
                Confirmed = false
            };

            _unitOfWork.FriendInvitationRepository.Add(friendInvitation);

            //wysłanoZaproszenieDoZajomych
            //otrzymanoZaprosznieDoznajmych

            if (await _unitOfWork.Complete())
            {
                var friendInvitationAcceptedEvent = new InviteUserToFriendsEvent()
                {
                    ObjectId = new FriendInvitationId(){ InviterUserId=friendInvitation.InviterUserId, InvitedUserId=friendInvitation.InvitedUserId },
                    NotificationRecipient = new SimpleUser() { UserEmail = friendInvitation.InvitedUserEmail, UserId = friendInvitation.InvitedUserId },
                    NotificationSender = new SimpleUser() { UserEmail = userEmail, UserId = userId },
                    EventType = "InviteUserToFriendsEvent",
                    FriendInvitationDto = _mapper.Map<FriendInvitationDtoGlobal>(friendInvitation),
                    UserWhoInvited = new SimpleUser() { UserEmail = userEmail, UserId = userId },
                    InvitedUser = new SimpleUser() { UserEmail = friendInvitation.InvitedUserEmail, UserId = friendInvitation.InvitedUserId },
                };
                await _messageBus.PublishMessage(friendInvitationAcceptedEvent);

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
            //var users = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            //var newOnlineMessagesUserWithFriendsEvent = new NewOnlineMessagesUserWithFriendsEvent() { NewOnlineUserChatFriends = users, NewOnlineUser = new SimpleUser() { UserEmail = userEmail, UserId = userId } };
            //await _messageBus.PublishMessage(newOnlineMessagesUserWithFriendsEvent, "new-online-messages-user-with-friends-queue");
            //??????????????????????????????????????????????????Po Co To
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
        //[HttpPost("AcceptFriendInvitation")]
        //public async Task<IActionResult> AcceptFriendInvitation(FriendInvitationDto friendInvitationDto)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User cannot be identified");
        //    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("User cannot be identified");

        //    var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, userId);

        //    if (friendInvitation == null)
        //        return BadRequest("invitation does not exist");

        //    friendInvitation.Confirmed = true;

        //    if (await _unitOfWork.Complete())
        //    {
        //        //
        //        //var users = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
        //        var friendInvitationAcceptedEvent = new FriendInvitationAcceptedEvent() 
        //        {
        //            EventType = "FriendInvitationAcceptedEvent",
        //            NotificationRecipient = new SimpleUser() { UserEmail = friendInvitation.InviterUserEmail, UserId = friendInvitation.InviterUserId },
        //            NotificationSender = new SimpleUser() { UserEmail = userEmail, UserId = userId },
        //            FriendInvitationDto = _mapper.Map<FriendInvitationDtoGlobal>(friendInvitation), 
        //            UserWhoAcceptedInvitation = new SimpleUser() { UserEmail = userEmail, UserId = userId},
        //            UserWhoseInvitationAccepted = new SimpleUser() { UserEmail = friendInvitation.InviterUserEmail, UserId = friendInvitation.InviterUserId },
        //        };
        //        await _messageBus.PublishMessage(friendInvitationAcceptedEvent, "friend-invitation-accepted-queue");
        //        //
        //        return Ok();
        //    };

        //    return BadRequest("The invitation cannot be confirmed.");
        //}
        [HttpPost("DeclineFriendInvitation")]
        public async Task<IActionResult> DeclineFriendInvitation(FriendInvitationId friendInvitationId)
        {
            var InitedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

            //var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, InitedUserId);

            //var friendInvitation = _mapper.Map<FriendInvitationDto, FriendInvitation>(friendInvitationDto);

            var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetInvitationAsync(friendInvitationId.InvitedUserId, friendInvitationId.InviterUserId);

            if (friendInvitation == null)
                return BadRequest("invitation does not exist");

            _unitOfWork.FriendInvitationRepository.Remove(friendInvitation);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("This invitation cannot be canceled.");
        }

        //var testt = await _unitOfWork.FriendInvitationRepository.GetOneByIdAsync(friendInvitationDto.InviterUserId, userId);
        [HttpPost("AcceptFriendInvitation")]
        public async Task<IActionResult> AcceptFriendInvitation(FriendInvitationId friendInvitationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User cannot be identified");
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("User cannot be identified");

            //var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, userId);
            var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetInvitationAsync(friendInvitationId.InvitedUserId, friendInvitationId.InviterUserId);

            if (friendInvitation == null)
                return BadRequest("invitation does not exist");

            if (friendInvitation.Confirmed)
                return BadRequest("invitation is already confirmed");

            friendInvitation.Confirmed = true;

            if (await _unitOfWork.Complete())
            {
                //
                //var users = friendsInvitationDtos.Select(x => x.InviterUserId == userId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
                var friendInvitationAcceptedEvent = new FriendInvitationAcceptedEvent()
                {
                    ObjectId = new FriendInvitationId() { InviterUserId = friendInvitation.InviterUserId, InvitedUserId = friendInvitation.InvitedUserId },
                    EventType = "FriendInvitationAcceptedEvent",
                    NotificationRecipient = new SimpleUser() { UserEmail = friendInvitation.InviterUserEmail, UserId = friendInvitation.InviterUserId },
                    NotificationSender = new SimpleUser() { UserEmail = userEmail, UserId = userId },
                    FriendInvitationDto = _mapper.Map<FriendInvitationDtoGlobal>(friendInvitation),
                    UserWhoAcceptedInvitation = new SimpleUser() { UserEmail = userEmail, UserId = userId },
                    UserWhoseInvitationAccepted = new SimpleUser() { UserEmail = friendInvitation.InviterUserEmail, UserId = friendInvitation.InviterUserId },
                };
                await _messageBus.PublishMessage(friendInvitationAcceptedEvent);
                //
                return Ok();
            };

            return BadRequest("The invitation cannot be confirmed.");
        }
        [HttpGet("test2")]
        public async Task<ActionResult<IEnumerable<UserFriendStatusToTheUser>>> test22([FromQuery] string searcherId, [FromQuery(Name = "idOfSearchedUsers")] string[] ids)
        {
            if (searcherId is null)
            {
                return BadRequest("User cannot be identified");
            }
            if (ids.Length == 0)
            {
                return BadRequest("You need to search for at least one id");
            }

            var usersStatus = await _unitOfWork.FriendInvitationRepository.GetFriendsStatusAsync(searcherId, ids);

            return Ok(usersStatus);
        }
    }
}


//[HttpPost("DeclineFriendInvitation")]
//public async Task<IActionResult> DeclineFriendInvitation(FriendInvitationDto friendInvitationDto)
//{
//    var InitedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

//    //var friendInvitation = await _unitOfWork.FriendInvitationRepository.GetFriendInvitation(friendInvitationDto.InviterUserId, InitedUserId);

//    var friendInvitation = _mapper.Map<FriendInvitationDto, FriendInvitation>(friendInvitationDto);

//    if (friendInvitation == null)
//        return BadRequest("invitation does not exist");

//    _unitOfWork.FriendInvitationRepository.Remove(friendInvitation);

//    if (await _unitOfWork.Complete())
//        return Ok();

//    return BadRequest("This invitation cannot be canceled.");
//}