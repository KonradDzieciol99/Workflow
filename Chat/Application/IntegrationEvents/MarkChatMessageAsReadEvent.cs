using MediatR;
using MessageBus;
using Chat.Infrastructure.Repositories;

namespace Chat.Application.IntegrationEvents;

public class MarkChatMessageAsReadEvent : IntegrationEvent
{
    public string ChatMessageId { get; set; }
    public DateTime ChatMessageDateRead { get; set; }
}
public class MarkChatMessageAsReadEventHandler : IRequestHandler<MarkChatMessageAsReadEvent>
{
    private IUnitOfWork _unitOfWork;

    public MarkChatMessageAsReadEventHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task Handle(MarkChatMessageAsReadEvent request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessagesRepository.GetAsync(request.ChatMessageId);

        message.MarkMessageAsRead(request.ChatMessageDateRead);

        await _unitOfWork.Complete();
    }
}