using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Socjal.API.Dto;
using Socjal.API.Repositories;
using StackExchange.Redis;
using System.Security.Claims;

namespace Socjal.API
{
    public class PresenceHub : Hub
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDatabase _redisDb;
        public PresenceHub(IConnectionMultiplexer connectionMultiplexer, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _connectionMultiplexer = connectionMultiplexer;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");
            await _redisDb.SetAddAsync(userEmail, Context.ConnectionId);


            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
            var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");
            await _redisDb.SetRemoveAsync(userEmail, Context.ConnectionId);

            var ConnectionLength = await _redisDb.SetLengthAsync(userEmail);
            if (ConnectionLength == 0)
            {
                await _redisDb.KeyDeleteAsync(userEmail);
                //string[] InvitedUsersConnectionIds = await GetInvitedUsers();
                //await Clients.Clients(InvitedUsersConnectionIds).SendAsync("UserIsOffline", userName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SentFriendInvitationNotification(string Id)
        {
            var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");

            //var Ids = await _redisDb.SetMembersAsync(email);

            //await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userName}");

            //Clients.User(Id).SendAsync("chat_notification", Ids);
            //Clients.co
            //Clients.
            //await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

        }
    }
}
