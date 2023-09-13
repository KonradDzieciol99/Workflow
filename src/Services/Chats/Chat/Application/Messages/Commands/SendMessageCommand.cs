using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Exceptions;
using Chat.Application.IntegrationEvents;
using Chat.Domain.Common.Exceptions;
using Chat.Domain.Entity;
using Chat.Domain.Services;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Messages.Commands;

public record SendMessageCommand(string RecipientUserId, string RecipientEmail, string Content) : IAuthorizationRequest
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
    private readonly IEventBusSender _azureServiceBusSender;
    private readonly IMessageService _messageService;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IEventBusSender azureServiceBusSender, IMessageService messageService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        this._messageService = messageService;
    }
    public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.GetUserId(), request.RecipientUserId) ?? throw new ChatDomainException("Friend request cannot be found.", new NotFoundException());

        var message = new Message(_currentUserService.GetUserId(),
                                  _currentUserService.GetUserEmail(),
                                  request.RecipientUserId,
                                  request.RecipientEmail,
                                  request.Content);

        _messageService.AddMessage(message, friendRequest);

        await _unitOfWork.Complete();

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
