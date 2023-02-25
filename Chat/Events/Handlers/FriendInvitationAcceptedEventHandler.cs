using Chat.Persistence;
using Chat.Repositories;
using MediatR;
using MessageBus.Events;

namespace Chat.Events.Handlers
{
    public class FriendInvitationAcceptedEventHandler : IRequestHandler<FriendInvitationAcceptedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FriendInvitationAcceptedEventHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }


        public async Task Handle(FriendInvitationAcceptedEvent request, CancellationToken cancellationToken)
        {



            return;
        }
    }
}
