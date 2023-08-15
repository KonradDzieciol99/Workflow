using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Chat.Infrastructure.Repositories;

namespace Chat.Application.IntegrationEvents.Handlers;

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