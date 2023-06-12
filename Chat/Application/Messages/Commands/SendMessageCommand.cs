using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.FriendRequests.Commands;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using Chat.Domain.Services;
using Chat.Domain.Entity;

namespace Chat.Application.Messages.Commands;

public record SendMessageCommand(string RecipientUserId, string RecipientEmail,string Content) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>() { new ShareFriendRequestRequirement(RecipientUserId) };
    }
}
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAzureServiceBusSender _azureServiceBusSender;
    private readonly IMessageService _messageService;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAzureServiceBusSender azureServiceBusSender, IMessageService messageService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        this._messageService = messageService;
    }
    public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.UserId, request.RecipientUserId);

        var message = new Message(_currentUserService.UserId,
                                  _currentUserService.UserEmail,
                                  request.RecipientUserId,
                                  request.RecipientEmail,
                                  request.Content);

        _messageService.AddMessage(message, friendRequest);
        
        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var @event = new ChatMessageAddedEvent(message.Id,
                                               message.SenderId,
                                               message.SenderEmail,
                                               message.RecipientId,
                                               message.RecipientEmail,
                                               message.Content,
                                               message.DateRead,
                                               message.MessageSent,
                                               message.SenderDeleted,
                                               message.RecipientDeleted);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
