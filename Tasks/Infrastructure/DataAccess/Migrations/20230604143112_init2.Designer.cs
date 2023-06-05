﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tasks.Infrastructure.DataAccess;

#nullable disable

namespace Tasks.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230604143112_init2")]
    partial class init2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Tasks.Domain.Entity.AppTask", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("TaskAssigneeMemberEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskAssigneeMemberId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskAssigneeMemberPhotoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskLeaderId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TaskLeaderId");

                    b.ToTable("AppTasks");
                });

            modelBuilder.Entity("Tasks.Domain.Entity.ProjectMember", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("Tasks.Domain.Entity.AppTask", b =>
                {
                    b.HasOne("Tasks.Domain.Entity.ProjectMember", "TaskLeader")
                        .WithMany("ConductedTasks")
                        .HasForeignKey("TaskLeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("TaskLeader");
                });

            modelBuilder.Entity("Tasks.Domain.Entity.ProjectMember", b =>
                {
                    b.Navigation("ConductedTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
