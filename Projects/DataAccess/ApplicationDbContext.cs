﻿using Microsoft.EntityFrameworkCore;
using Projects.Entity;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Projects.DataAccess
{
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
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

                opt.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<ProjectMember>(opt =>
            {
                opt.HasKey(x => x.Id);

                opt.HasOne<Project>(x => x.LedProject)
                .WithOne(x => x.Leader)
                .HasForeignKey<Project>(x => x.LeaderId)
                .IsRequired(false);

                opt.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }
    }

}






