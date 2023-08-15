using Chat.Domain.Entity;

namespace Chat.Infrastructure.Repositories;

public interface IMessagesRepository
{
    void Add(Message entity);
    void AddRange(IEnumerable<Message> entities);
    //Task<Message?> GetAsync(string sourceUserId, string targetUserId);
    Task<Message?> GetAsync(string messageId);
    Task<List<Message>> GetMessageThreadAsync(string sourceUserEmail, string targetUserEmail, int skip, int take);
    void Remove(Message entity);
    Task<List<string>> GetUnreadMessagesUserEmails(string recipientId);
}