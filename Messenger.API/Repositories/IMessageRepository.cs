using Socjal.API.Entity;

namespace Socjal.API.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        public Task<IEnumerable<Message>> GetMessageThreadAsync(string currentUserEmail, string recipientUserEmail);

    }
}
