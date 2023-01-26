using Messenger.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace Messenger.API
{
    [Authorize]
    public class MessagesHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDb;

        public MessagesHub(IConnectionMultiplexer connectionMultiplexer, IUnitOfWork unitOfWork)
        {
            this._connectionMultiplexer = connectionMultiplexer;
            this._redisDb = _connectionMultiplexer.GetDatabase();

        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
            if (string.IsNullOrEmpty(recipientEmail)) 
                throw new HubException("User cannot be identified"); 
            var SenderEmail = Context?.User?.Identity?.Name ?? throw new HubException("User cannot be identified");
            
            var groupName = GetGroupName(Context.User.Identity.Name, recipientEmail);
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
            var SenderEmail = Context?.User?.Identity?.Name ?? throw new HubException("User cannot be identified");

            var groupName = GetGroupName(Context.User.Identity.Name, recipientEmail);

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
    }
}
