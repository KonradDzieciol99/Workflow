using Chat.Entity;

namespace Chat.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        public Task<IEnumerable<Message>> GetMessageThreadAsync(string currentUserEmail, string recipientUserEmail);

    }
}
