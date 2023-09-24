using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entity;

namespace Notification.Infrastructure.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<AppNotification> AppNotification { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppNotification>(opt =>
        {
            opt.HasKey(x => x.Id);

            opt.Property(x => x.Id).ValueGeneratedOnAdd();
        });
    }
}
