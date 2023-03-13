using AutoMapper;
using AutoMapper.Internal;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Chat.Services;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureServiceBusSender _messageBus;
        private readonly IIdentityServerService _identityServerService;

        public ChatController(IUnitOfWork unitOfWork, IMapper mapper, IAzureServiceBusSender messageBus, IIdentityServerService identityServerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._messageBus = messageBus;
            this._identityServerService = identityServerService;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (senderId is null || senderEmail is null)
                return BadRequest("User cannot be identified");

            //var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(senderId);
            //var recipient = await _unitOfWork.UserRepository.GetUserByEmailAsync(createMessageDto.RecipientEmail);
            //var token = await HttpContext.GetTokenAsync("access_token");//"Bearer", 
            //var recipient = await _identityServerService.CheckIfUserExistsAsync<UserDto?>(createMessageDto.RecipientEmail, token);



            if (senderEmail == createMessageDto.RecipientEmail)
            {
                return BadRequest("You can't invite yourself.");
            }

            var invitation = await _unitOfWork.FriendInvitationRepository.GetInvitationAsync(senderId, createMessageDto.RecipientEmail);

            if (invitation is null || invitation.Confirmed==false)
            {
                return BadRequest("You must be friends with him to chat with him.");
            }
            //if (recipient is null)
            //{
            //    return BadRequest("The specified user do not exist.");
            //}
            //if (await _unitOfWork.FriendInvitationRepository.checkIfExistsAsync(senderId, recipient.Id))
            //{
            //    return BadRequest("You must be friends with him to chat with him.");
            //}

            var message = new Message
            {
                //Sender = sender,
                SenderId = senderId,
                //Recipient = recipient,
                RecipientId = invitation.InviterUserId == senderId ? invitation.InvitedUserId : invitation.InviterUserId,
                SenderEmail = senderEmail,
                RecipientEmail = invitation.InviterUserEmail == senderId ? invitation.InvitedUserEmail : invitation.InviterUserEmail,
                Content = createMessageDto.Content
            }; 

            _unitOfWork.MessageRepository.Add(message);

            if (await _unitOfWork.Complete())
            {
                //OrderStatusChangedToAwaitingValidationIntegrationEvent
                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = localUserRegisterSuccessEvent.LocalUserEmail, Id = localUserRegisterSuccessEvent.IdentityUserId };

                var SendMessageToSignalREvent = _mapper.Map<SendMessageToSignalREvent>(message);
                SendMessageToSignalREvent.NotificationSender = new SimpleUser() { UserEmail = senderEmail, UserId = senderId };
                SendMessageToSignalREvent.NotificationRecipient = new SimpleUser() { UserEmail = message.RecipientEmail, UserId = message.RecipientId};
                await _messageBus.PublishMessage(SendMessageToSignalREvent, "send-message-to-signalr");
                return Ok();
            }

            return BadRequest("User cannot be invited.");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread([FromQuery] string recipientEmail)
        {
            var senderEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (senderEmail is null)
                return BadRequest("User cannot be identified");

            var messages = await _unitOfWork.MessageRepository.GetMessageThreadAsync(senderEmail, recipientEmail);
            var messagesDto = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageDto>>(messages);

            if (_unitOfWork.HasChanges())
            {
                if (!await _unitOfWork.Complete()) { return BadRequest("Failed to mark as read"); }
            }

            return Ok(messagesDto);
        }
        //public async Task SendMessage(CreateMessageDto createMessageDto)
        //{
        //    var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        //    var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

        //    if (SenderEmail == createMessageDto.RecipientEmail)
        //        throw new HubException("You cannot send messages to yourself");

        //    var sender = await _unitOfWork.UserRepository.GetUserByEmailAsync(SenderEmail);
        //    var recipient = await _unitOfWork.UserRepository.GetUserByEmailAsync(createMessageDto.RecipientEmail);

        //    if (recipient == null || sender == null) throw new HubException("Not found user");

        //    var message = new Message
        //    {

        //        Sender = sender,
        //        SenderId = sender.Id,
        //        Recipient = recipient,
        //        RecipientId = recipient.Id,
        //        SenderEmail = sender.Email,
        //        RecipientEmail = recipient.Email,
        //        Content = createMessageDto.Content
        //    };

        //    var groupName = GetGroupName(sender.Email, recipient.Email);
        //    var values = await _redisDb.HashValuesAsync(groupName);
        //    if (values.Contains(recipient.Email))
        //        message.DateRead = DateTime.UtcNow;


        //    //else
        //    //{
        //    //    var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
        //    //    if (connections != null)
        //    //    {
        //    //        await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
        //    //            new { username = sender.UserName, knownAs = sender.KnownAs });
        //    //    }
        //    //}

        //    _unitOfWork.MessageRepository.Add(message);

        //    if (!await _unitOfWork.Complete())
        //    {
        //        throw new HubException("some errors occurred");
        //    }

        //    await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

        //}


    }
}
