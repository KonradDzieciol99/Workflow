﻿using AutoMapper;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
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

        public FriendInvitationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        [HttpGet("GetAllFriends")]
        public async Task<ActionResult<UserDto>> GetAllFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("User cannot be identified");

            var friendsInvitation = await _unitOfWork.FriendInvitationRepository.GetAllFriends(userId);

            if (friendsInvitation == null)
            {
                return NotFound();
            }

            var friendsInvitationDto = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);

            return Ok(friendsInvitationDto);
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
                return Ok();

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