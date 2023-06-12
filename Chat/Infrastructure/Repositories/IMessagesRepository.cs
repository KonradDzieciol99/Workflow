using Chat.Domain.Entity;

namespace Chat.Infrastructure.Repositories
{
    public interface IMessagesRepository
    {
        void Add(Message entity);
        void AddRange(IEnumerable<Message> entities);
        Task<Message?> GetAsync(string sourceUserId, string targetUserId);
        Task<List<Message>> GetMessageThreadAsync(string sourceUserEmail, string targetUserEmail);
        void Remove(Message entity);
    }
}