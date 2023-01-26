

using Messenger.API.Models;
using Messenger.API.Persistence;

namespace Messenger.API.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
