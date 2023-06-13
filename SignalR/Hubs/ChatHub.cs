using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using StackExchange.Redis;
using System.Security.Claims;

namespace SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;
        private readonly IDatabase _redisDb;

        public ChatHub(IConnectionMultiplexer connectionMultiplexer)//, IUnitOfWork unitOfWork, IMapper mapper
        {
            _connectionMultiplexer = connectionMultiplexer;
            //_unitOfWork = unitOfWork;
            //_mapper = mapper;
            _redisDb = _connectionMultiplexer.GetDatabase();

        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
            if (string.IsNullOrEmpty(recipientEmail))
                throw new HubException("User cannot be identified");
            var UserEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var groupName = GetGroupName(UserEmail, recipientEmail);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await _redisDb.HashSetAsync(groupName, Context.ConnectionId, recipientEmail);
            await _redisDb.HashSetAsync(groupName, Context.ConnectionId, UserEmail);

            List<string> groupMembers = new() { UserEmail };

            var values = await _redisDb.HashValuesAsync(groupName);
            if (values.Contains(recipientEmail))
                groupMembers.Add(recipientEmail);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);

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

            await base.OnDisconnectedAsync(ex);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        public async Task UserIsTyping()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
            if (string.IsNullOrEmpty(recipientEmail))
                throw new HubException("User cannot be identified");
            var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

            var groupName = GetGroupName(userEmail, recipientEmail);

            //List<ChatGroupMember> groupMembers = new() { new ChatGroupMember() { UserEmail = userEmail, IsTyping = true } };

            await Clients.Group(groupName).SendAsync("UserIsTyping", userEmail);
        }
        //public async Task UserIsTyping()
        //{
        //    var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        //    var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
        //    if (string.IsNullOrEmpty(recipientEmail))
        //        throw new HubException("User cannot be identified");
        //    var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

        //    var groupName = GetGroupName(userEmail, recipientEmail);

        //    List<ChatGroupMember> groupMembers = new() { new ChatGroupMember() { UserEmail = userEmail, IsTyping = true } };

        //    var values = await _redisDb.HashValuesAsync(groupName);
        //    if (values.Contains(recipientEmail))
        //        groupMembers.Add(new ChatGroupMember() { UserEmail = recipientEmail });

        //    await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);
        //}
        //public async Task UserStoppedTyping()
        //{
        //    var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        //    var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
        //    if (string.IsNullOrEmpty(recipientEmail))
        //        throw new HubException("User cannot be identified");
        //    var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

        //    var groupName = GetGroupName(userEmail, recipientEmail);

        //    List<ChatGroupMember> groupMembers = new() { new ChatGroupMember() { UserEmail = userEmail} };

        //    var values = await _redisDb.HashValuesAsync(groupName);
        //    if (values.Contains(recipientEmail))
        //        groupMembers.Add(new ChatGroupMember() { UserEmail = recipientEmail });

        //    await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);
        //}
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


        ////    else
        ////    {
        ////        var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
        ////        if (connections != null)
        ////        {
        ////            await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
        ////                new { username = sender.UserName, knownAs = sender.KnownAs
        ////});
        ////        }
        ////    }

        //    _unitOfWork.MessageRepository.Add(message);

        //    if (!await _unitOfWork.Complete())
        //    {
        //        throw new HubException("some errors occurred");
        //    }

        //    await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

        //}
    }
}
