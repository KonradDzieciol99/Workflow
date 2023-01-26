

using Messenger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.API.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>(opt =>
            {
                opt.HasKey(x => x.Id);
            });
        }
    }
}
