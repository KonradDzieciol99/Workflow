using AutoMapper;
using Chat.Entity;
using Chat.Repositories;
using Mango.MessageBus;
using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;

namespace Chat.Events.Handlers
{
    public class NewOnlineUserEventHandler : IRequestHandler<NewOnlineUserEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureServiceBusSender _messageBus;

        public NewOnlineUserEventHandler(IUnitOfWork unitOfWork,IMapper mapper, IAzureServiceBusSender messageBus)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._messageBus = messageBus;
        }
        public async Task Handle(NewOnlineUserEvent request, CancellationToken cancellationToken)
        {
            var friendsInvitation = await _unitOfWork.FriendInvitationRepository.GetAllFriends(request.NewOnlineUser.UserId);

            if (friendsInvitation == null) { throw new ArgumentNullException("TODO"); }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<Dto.FriendInvitationDto>>(friendsInvitation);

            var onlineUsers = friendsInvitationDtos.Select(x => x.InviterUserId == request.NewOnlineUser.UserId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            var newOnlineUserWithFriendsEvent = new NewOnlineUserWithFriendsEvent() { NewOnlineUserChatFriends = onlineUsers, NewOnlineUser = new SimpleUser() { UserEmail = request.NewOnlineUser.UserEmail, UserId = request.NewOnlineUser.UserId } };
            await _messageBus.PublishMessage(newOnlineUserWithFriendsEvent);
        }
    }
}
