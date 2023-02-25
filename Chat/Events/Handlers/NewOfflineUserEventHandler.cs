using AutoMapper;
using Chat.Dto;
using Chat.Entity;
using Chat.Repositories;
using Mango.MessageBus;
using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;

namespace Chat.Events.Handlers
{
    public class NewOfflineUserEventHandler : IRequestHandler<NewOfflineUserEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureServiceBusSender _messageBus;

        public NewOfflineUserEventHandler(IUnitOfWork unitOfWork,IMapper mapper, IAzureServiceBusSender messageBus)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._messageBus = messageBus;
        }
        public async Task Handle(NewOfflineUserEvent request, CancellationToken cancellationToken)
        {
            var friendsInvitation = await _unitOfWork.FriendInvitationRepository.GetAllFriends(request.User.UserId);

            if (friendsInvitation == null) { throw new ArgumentNullException("TODO"); }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);
            var users = friendsInvitationDtos.Select(x => x.InviterUserId == request.User.UserId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });

            var newOfflineUserWithFriendsEvent = new NewOfflineUserWithFriendsEvent() { UserChatFriends = users, User = new SimpleUser() { UserEmail = request.User.UserEmail, UserId = request.User.UserId } };
            await _messageBus.PublishMessage(newOfflineUserWithFriendsEvent, "new-offline-user-with-friends-queue");
        }
    }
}
