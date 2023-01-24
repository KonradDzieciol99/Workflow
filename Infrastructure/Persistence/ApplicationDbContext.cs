

using Domain.Entities;
using Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<RefreshToken>(opt =>
            {
                opt.HasKey(x => x.Id);

                opt.HasIndex(x => x.Token).IsUnique();

                opt.HasOne(x => x.AppUser)
                   .WithMany(x => x.RefreshTokens)
                   .HasForeignKey(x => x.UserId);
            });
        }
    }
}
