using Chat.Entity;
using Chat.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MessageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public async Task<IEnumerable<Message>> GetMessageThreadAsync(string currentUserEmail, string recipientUserEmail)
        {
            var query = applicationDbContext.Messages
                .Where(
                    m => m.RecipientEmail == currentUserEmail && m.RecipientDeleted == false &&
                    m.SenderEmail == recipientUserEmail ||
                    m.RecipientEmail == recipientUserEmail && m.SenderDeleted == false &&
                    m.SenderEmail == currentUserEmail
                )
                .OrderBy(m => m.MessageSent)
                .AsQueryable();


            var unreadMessages = query.Where(m => m.DateRead == null
                && m.RecipientEmail == currentUserEmail).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return await query.ToListAsync();
        }
    }
}
