using Chat.Domain.Entity;
using Chat.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Repositories;

public class MessagesRepository : IMessagesRepository
{
    private readonly ApplicationDbContext _dbContext;
    public MessagesRepository(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }
    public void Add(Message entity)
    {
        _dbContext.Messages.Add(entity);
    }
    public void AddRange(IEnumerable<Message> entities)
    {
        _dbContext.Messages.AddRange(entities);
    }
    public void Remove(Message entity)
    {
        _dbContext.Messages.Remove(entity);
    }
    public async Task<Message?> GetAsync(string messageId)
    {
        return await _dbContext.Messages.FindAsync(messageId);
    }
    public async Task<List<Message>> GetMessageThreadAsync(string sourceUserEmail, string targetUserEmail, int skip, int take)
    {
        return await _dbContext.Messages.OrderByDescending(m => m.MessageSent)
                                        .Where(
                                            m => m.RecipientEmail == sourceUserEmail && m.RecipientDeleted == false &&
                                            m.SenderEmail == targetUserEmail ||
                                            m.RecipientEmail == targetUserEmail && m.SenderDeleted == false &&
                                            m.SenderEmail == sourceUserEmail
                                        )
                                        .Skip(skip)
                                        .Take(take)
                                        .ToListAsync();
    }
    public async Task<List<string>> GetUnreadMessagesUserEmails(string recipientId)
    {
        return await _dbContext.Messages
            .Where(m => m.RecipientId == recipientId && m.DateRead == null)
            .Select(m => m.SenderEmail)
            .ToListAsync();
    }
}
