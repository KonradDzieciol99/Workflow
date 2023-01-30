using Microsoft.EntityFrameworkCore;
using Socjal.API.Entity;

namespace Socjal.API.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>(opt =>
            {
                opt.HasKey(x => x.Id);
            });
            builder.Entity<User>(opt =>
            {
                opt.HasKey(x => x.Id);
            });

            builder.Entity<Message>(opt =>
            {
                opt.HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

                opt.HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
            });




        }
    }
}
