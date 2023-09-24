using Chat.Domain.Entity;

namespace Chat.Domain.Services;

public interface IMessageService
{
    void AddMessage(Message message, FriendRequest friendRequest);
}
