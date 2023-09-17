using Microsoft.EntityFrameworkCore;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Infrastructure.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(opt =>
        {
            opt.HasKey(x => x.Id);

            opt.HasMany<ProjectMember>(x => x.ProjectMembers)
            .WithOne(x => x.MotherProject)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            opt.Property(x => x.Id).ValueGeneratedNever();
        });

        builder.Entity<ProjectMember>(opt =>
        {
            opt.HasKey(x => x.Id);

            opt.Property(x => x.Id).ValueGeneratedOnAdd();
        });
    }
}






