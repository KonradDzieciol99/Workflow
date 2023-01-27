using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Socjal.API.Models;
using Socjal.API.Repositories;
using StackExchange.Redis;
using System.Security.Claims;

namespace Socjal.API
{
    [Authorize]
    public class MessagesHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabase _redisDb;

        public MessagesHub(IConnectionMultiplexer connectionMultiplexer, IUnitOfWork unitOfWork)
        {
            _connectionMultiplexer = connectionMultiplexer;
            this._unitOfWork = unitOfWork;
            _redisDb = _connectionMultiplexer.GetDatabase();

        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
            if (string.IsNullOrEmpty(recipientEmail))
                throw new HubException("User cannot be identified");
            var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var groupName = GetGroupName(SenderEmail, recipientEmail);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await _redisDb.HashSetAsync(groupName, Context.ConnectionId, recipientEmail);

            List<string> groupMembers = new() { SenderEmail };

            var values = await _redisDb.HashValuesAsync(groupName);
            if (values.Contains(recipientEmail))
                groupMembers.Add(recipientEmail);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", SenderEmail);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
            if (string.IsNullOrEmpty(recipientEmail))
                throw new HubException("User cannot be identified");
            var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var groupName = GetGroupName(SenderEmail, recipientEmail);

            await _redisDb.HashDeleteAsync(groupName, Context.ConnectionId);

            List<string> groupMembers = new();

            var values = await _redisDb.HashValuesAsync(groupName);
            if (values.Contains(recipientEmail))
                groupMembers.Add(recipientEmail);
            if (values.Contains(SenderEmail))
                groupMembers.Add(SenderEmail);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);

            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name); //https://stackoverflow.com/questions/23854979/signalr-is-it-necessary-to-remove-the-connection-id-from-group-ondisconnect

            await base.OnDisconnectedAsync(ex);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        //public async Task SendMessage(CreateMessageDto createMessageDto)
        //{
        //    var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        //    var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

        //    if (SenderEmail == createMessageDto.RecipientUsername)
        //        throw new HubException("You cannot send messages to yourself");

        //    var sender = await _unitOfWork.MessageRepository.GetUserByUsernameAsync(username);
        //    var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //    if (recipient == null) throw new HubException("Not found user");

        //    var message = new Message
        //    {
        //        SenderId = SenderId
        //        Sender = sender,
        //        Recipient = recipient,
        //        SenderUsername = sender.UserName,
        //        RecipientUsername = recipient.UserName,
        //        Content = createMessageDto.Content
        //    };

        //    var groupName = GetGroupName(sender.UserName, recipient.UserName);

        //    var group = await _uow.MessageRepository.GetMessageGroup(groupName);

        //    if (group.Connections.Any(x => x.Username == recipient.UserName))
        //    {
        //        message.DateRead = DateTime.UtcNow;
        //    }
        //    else
        //    {
        //        var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
        //        if (connections != null)
        //        {
        //            await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
        //                new { username = sender.UserName, knownAs = sender.KnownAs });
        //        }
        //    }

        //    _uow.MessageRepository.AddMessage(message);

        //    if (await _uow.Complete())
        //    {
        //        await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        //    }
        //}
    }
}
