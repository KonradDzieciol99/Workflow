using Microsoft.EntityFrameworkCore;
using Tasks.Domain.Entity;

namespace Tasks.Infrastructure.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppTask> AppTasks { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppTask>(opt =>
            {
                opt.HasKey(x => x.Id);

                //opt.HasMany<ProjectMember>(x => x.ProjectMembers)
                //.WithOne(x => x.MotherProject)
                //.HasForeignKey(x => x.ProjectId)
                //.OnDelete(DeleteBehavior.Cascade);

                opt.Property(x => x.Id).ValueGeneratedOnAdd();

                opt.HasOne(x => x.TaskLeader)
                   .WithMany(x => x.ConductedTasks)
                   .HasForeignKey(x => x.TaskLeaderId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);///tu może być błąd !!!
            });

            builder.Entity<ProjectMember>(opt =>
            {


                opt.HasKey(x => x.Id);

                opt.Property(x => x.Id).ValueGeneratedNever();
            });

        }
    }

}






