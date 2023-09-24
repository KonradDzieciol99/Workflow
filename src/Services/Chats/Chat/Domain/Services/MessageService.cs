using Chat.Domain.Common.Exceptions;
using Chat.Domain.Entity;
using Chat.Infrastructure.Repositories;

namespace Chat.Domain.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public void AddMessage(Message message, FriendRequest friendRequest)
    {
        if (!friendRequest.Confirmed)
            throw new ChatDomainException("Request must be confirmed");

        _unitOfWork.MessagesRepository.Add(message);
    }
}
