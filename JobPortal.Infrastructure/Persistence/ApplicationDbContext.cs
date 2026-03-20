using System;
using System.Collections.Generic;
using System.Text;



using JobPortal.Domain.Entities;

using Microsoft.EntityFrameworkCore;
namespace JobPortal.Infrastructure.Persistence
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        public DbSet<Applicant> Applicants => Set<Applicant>();
        public DbSet<Company> Companies => Set<Company>();

        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> Applications => Set<JobApplication>();
        public DbSet<SavedJob> SavedJobs => Set<SavedJob>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Unique email for Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Enums stored as strings
            modelBuilder.Entity<Job>()
                .Property(j => j.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Job>()
                .Property(j => j.RequiredExperience)
                .HasConversion<string>();

            modelBuilder.Entity<Applicant>()
                .Property(a => a.ExperienceLevel)
                .HasConversion<string>();

            modelBuilder.Entity<JobApplication>()
                .Property(a => a.Status)
                .HasConversion<string>();
        }
    }
}