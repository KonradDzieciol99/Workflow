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
    //public async Task<Message?> GetAsync(string sourceUserId, string targetUserId)
    //{
    //    return await _dbContext.Messages.FindAsync(sourceUserId, targetUserId);
    //}
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
            .Where(m => m.RecipientId == recipientId && m.DateRead == null)//&& !m.RecipientDeleted
            .Select(m=>m.SenderEmail)
            .ToListAsync();
    }

    //public async Task<IEnumerable<Message>> GetMessageThreadAsync(string currentUserEmail, string recipientUserEmail)
    //{
    //    var query = applicationDbContext.Messages
    //        .Where(
    //            m => m.RecipientEmail == currentUserEmail && m.RecipientDeleted == false &&
    //            m.SenderEmail == recipientUserEmail ||
    //            m.RecipientEmail == recipientUserEmail && m.SenderDeleted == false &&
    //            m.SenderEmail == currentUserEmail
    //        )
    //        .OrderBy(m => m.MessageSent)
    //        .AsQueryable();


    //    var unreadMessages = query.Where(m => m.DateRead == null
    //        && m.RecipientEmail == currentUserEmail).ToList();

    //    if (unreadMessages.Any())
    //    {
    //        foreach (var message in unreadMessages)
    //        {
    //            message.DateRead = DateTime.UtcNow;
    //        }
    //    }

    //    return await query.ToListAsync();
    //}       DO PRZETESTOWAIA !!!
}
