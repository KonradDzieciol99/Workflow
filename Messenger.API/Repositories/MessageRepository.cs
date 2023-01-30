using Socjal.API.Entity;
using Socjal.API.Persistence;

namespace Socjal.API.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
