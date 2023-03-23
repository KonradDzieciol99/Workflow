using Chat.Repositories;
using MediatR;
using MessageBus.Events;

namespace Chat.Events.Handlers
{
    public class MarkChatMessageAsReadEventHandler : IRequestHandler<MarkChatMessageAsReadEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkChatMessageAsReadEventHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task Handle(MarkChatMessageAsReadEvent request, CancellationToken cancellationToken)
        {
            var message = await _unitOfWork.MessageRepository.GetOneAsync(x => x.Id == request.ObjectId.ToString());

            if (message is null)
                throw new ArgumentNullException($"I can't find the chat message from event:{request}");

            message.DateRead = request.DateRead;

            if (!_unitOfWork.HasChanges())
                throw new Exception("Failed to mark as read");
            if (!await _unitOfWork.Complete())
                throw new Exception("Failed to mark as read");
        }
    }
}
