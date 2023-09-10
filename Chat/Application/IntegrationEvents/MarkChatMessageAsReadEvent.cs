using MediatR;
using MessageBus;
using Chat.Infrastructure.Repositories;

namespace Chat.Application.IntegrationEvents;

public record MarkChatMessageAsReadEvent(string ChatMessageId, DateTime ChatMessageDateRead) : IntegrationEvent;

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