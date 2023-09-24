using Chat.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FriendRequest>(opt =>
        {
            opt.HasKey(k => new { k.InviterUserId, k.InvitedUserId });
        });

        builder.Entity<Message>(opt =>
        {
            opt.HasKey(x => x.Id);

            opt.Property(x => x.Id).ValueGeneratedOnAdd();
        });
    }
}
