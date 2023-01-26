using Messenger.API.Persistence;

namespace Messenger.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IMessageRepository MessageRepository => new MessageRepository(_applicationDbContext);


        public async Task<bool> Complete()
        {
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }
        public bool HasChanges()
        {
            _applicationDbContext.ChangeTracker.DetectChanges();
            var changes = _applicationDbContext.ChangeTracker.HasChanges();

            return changes;
        }
    }
}
