using AutoMapper;
using AutoMapper.Internal;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
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
        private readonly IMessageBus _messageBus;

        public ChatController(IUnitOfWork unitOfWork, IMapper mapper,IMessageBus messageBus)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._messageBus = messageBus;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (senderId is null)
                return BadRequest("User cannot be identified");
        
            var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(senderId);
            var recipient = await _unitOfWork.UserRepository.GetUserByEmailAsync(createMessageDto.RecipientEmail);

            if (sender is null || recipient is null)
            {
                return BadRequest("The specified users do not exist.");
            }
            if (await _unitOfWork.FriendInvitationRepository.checkIfExistsAsync(senderId, recipient.Id))
            {
                return BadRequest("You must be friends with him to chat with him.");
            }
            if (sender.Id == recipient.Id)
            {
                return BadRequest("You can't invite yourself.");
            }
            var message = new Message
            {
                Sender = sender,
                SenderId = sender.Id,
                Recipient = recipient,
                RecipientId = recipient.Id,
                SenderEmail = sender.Email,
                RecipientEmail = recipient.Email,
                Content = createMessageDto.Content
            };

            _unitOfWork.MessageRepository.Add(message);

            if (await _unitOfWork.Complete())
            {
                //OrderStatusChangedToAwaitingValidationIntegrationEvent
                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = localUserRegisterSuccessEvent.LocalUserEmail, Id = localUserRegisterSuccessEvent.IdentityUserId };
                var SendMessageToSignalREvent = _mapper.Map<SendMessageToSignalREvent>(message);
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
