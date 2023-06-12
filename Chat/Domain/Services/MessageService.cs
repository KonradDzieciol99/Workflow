using Chat.Domain.Common.Exceptions;
using Chat.Domain.Entity;
using Chat.Infrastructure.Repositories;

namespace Chat.Domain.Services;

public class MessageService : IMessageService
{
    private readonly IMessagesRepository _messagesRepository;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _messagesRepository = unitOfWork.MessagesRepository ?? throw new ArgumentNullException(nameof(unitOfWork.MessagesRepository));
    }
    public void AddMessage(Message message, FriendRequest friendRequest)
    {
        if (!friendRequest.Confirmed)
            throw new ChatDomainException("Request must be confirmed");

        _messagesRepository.Add(message);
    }
}
