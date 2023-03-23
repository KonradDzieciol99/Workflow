using Microsoft.EntityFrameworkCore;
using Chat.Entity;

namespace Chat.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<FriendInvitation> FriendsInvitation { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //builder.Entity<User>(opt =>
            //{
            //    opt.HasKey(x => x.Id);
            //});

            builder.Entity<Message>(opt =>
            {
                opt.HasKey(x => x.Id);
                opt.Property(x => x.Id).ValueGeneratedOnAdd();
                //opt.HasOne(m => m.Sender)
                //.WithMany(u => u.MessagesSent)
                //.HasForeignKey(m => m.SenderId)
                //.OnDelete(DeleteBehavior.Restrict);

                //opt.HasOne(m => m.Recipient)
                //.WithMany(u => u.MessagesReceived)
                //.HasForeignKey(m => m.RecipientId)
                //.OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<FriendInvitation>(opt =>
            {
                opt.HasKey(k => new { k.InviterUserId, k.InvitedUserId });

                opt.Property(u => u.Confirmed).HasDefaultValue(false);

                //opt.HasOne(s => s.InviterUser)
                //    .WithMany(l => l.FriendInvitationSent)
                //    .HasForeignKey(s => s.InviterUserId)
                //    .OnDelete(DeleteBehavior.Restrict);

                //opt.HasOne(s => s.InvitedUser)
                //    .WithMany(l => l.FriendInvitationRecived)
                //    .HasForeignKey(s => s.InvitedUserId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });









        }

    }
}
