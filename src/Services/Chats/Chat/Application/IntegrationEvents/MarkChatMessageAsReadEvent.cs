using MediatR;
using MessageBus;
using Chat.Infrastructure.Repositories;

namespace Chat.Application.IntegrationEvents;

public record MarkChatMessageAsReadEvent(string ChatMessageId, DateTime ChatMessageDateRead)
    : IntegrationEvent;

public class MarkChatMessageAsReadEventHandler : IRequestHandler<MarkChatMessageAsReadEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkChatMessageAsReadEventHandler(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task Handle(
        MarkChatMessageAsReadEvent request,
        CancellationToken cancellationToken
    )
    {
        var message =
            await _unitOfWork.MessagesRepository.GetAsync(request.ChatMessageId)
            ?? throw new InvalidOperationException("Such a message does not exist");

        message.MarkMessageAsRead(request.ChatMessageDateRead);

        await _unitOfWork.Complete();
    }
}
