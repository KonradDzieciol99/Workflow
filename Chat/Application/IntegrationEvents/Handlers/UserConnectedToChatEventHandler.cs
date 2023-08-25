using AutoMapper;
using Chat.Application.Common.Models;
using Chat.Infrastructure.Repositories;
using MediatR;
using MessageBus;
using MessageBus.Events;

namespace Chat.Application.IntegrationEvents.Handlers;

public class UserConnectedToChatEventHandler : IRequestHandler<UserConnectedToChatEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;
    private readonly IMapper _mapper;

    public UserConnectedToChatEventHandler(IUnitOfWork unitOfWork,
                                           IAzureServiceBusSender azureServiceBusSender,
                                           IMapper mapper)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = azureServiceBusSender;
        this._mapper = mapper;
    }
    public async Task Handle(UserConnectedToChatEvent request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.MessagesRepository.GetMessageThreadAsync(request.ConnectedUser.Email, request.RecipientEmail, 0, 15);

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientEmail == request.ConnectedUser.Email).ToList();

        if (unreadMessages.Any())
            foreach (var message in unreadMessages)
                message.MarkMessageAsRead();

        if (_unitOfWork.HasChanges())
            await _unitOfWork.Complete();

        var @event = new UserConnectedToChatResponseEvent(request.ConnectedUser, request.RecipientEmail, _mapper.Map<List<MessageDto>>(messages));

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
