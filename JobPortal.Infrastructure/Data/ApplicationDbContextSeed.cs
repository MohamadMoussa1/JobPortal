using JobPortal.Domain.Entities;
using JobPortal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Infrastructure.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed Roles
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "Company" },
                    new Role { Name = "Applicant" }
                };

                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }

            // Seed Permissions
            if (!context.Permissions.Any())
            {
                var permissions = new List<Permission>
                {
                    // Admin
                    new Permission { Name = "ManageUsers", DisplayName = "Manage Users", Description = "Create, update and delete users" },
                    new Permission { Name = "ManageCompanies", DisplayName = "Manage Companies", Description = "Approve and manage companies" },
                    new Permission { Name = "ManageJobs", DisplayName = "Manage Jobs", Description = "Manage all job postings" },
                    new Permission { Name = "ManageApplications", DisplayName = "Manage Applications", Description = "Manage job applications" },
                    new Permission { Name = "ViewReports", DisplayName = "View Reports", Description = "Access reports and analytics" },

                    // Company
                    new Permission { Name = "CreateJob", DisplayName = "Create Job", Description = "Post new job" },
                    new Permission { Name = "EditJob", DisplayName = "Edit Job", Description = "Edit own job postings" },
                    new Permission { Name = "DeleteJob", DisplayName = "Delete Job", Description = "Delete own job postings" },
                    new Permission { Name = "ViewApplicants", DisplayName = "View Applicants", Description = "View applicants for jobs" },
                    new Permission { Name = "ViewJobs", DisplayName = "View Jobs", Description = "View job listings" },
                    new Permission { Name = "UpdateProfile", DisplayName = "Update Profile", Description = "Update profile information" },

                    // Applicant
                    new Permission { Name = "ApplyJob", DisplayName = "Apply to Job", Description = "Apply for jobs" },
                    new Permission { Name = "SaveJob", DisplayName = "Save Job", Description = "Save jobs for later" },
                    new Permission { Name = "ViewCompanies", DisplayName = "View Companies", Description = "Browse companies" },
                    new Permission { Name = "ViewApplications", DisplayName = "View Applications", Description = "View own applications" }
                };

                context.Permissions.AddRange(permissions);
                await context.SaveChangesAsync();
            }

            // Seed RolePermissions
            if (!context.RolePermissions.Any())
            {
                var admin = await context.Roles.FirstAsync(r => r.Name == "Admin");
                var company = await context.Roles.FirstAsync(r => r.Name == "Company");
                var applicant = await context.Roles.FirstAsync(r => r.Name == "Applicant");

                var permissions = await context.Permissions.ToListAsync();

                // Admin → all permissions
                foreach (var p in permissions.Where(p => p.Name.StartsWith("Manage") || p.Name == "ViewReports"))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = admin.Id,
                        PermissionId = p.Id
                    });
                }

                // Company permissions
                foreach (var p in permissions.Where(p =>
                    p.Name == "CreateJob" || p.Name == "EditJob" ||
                    p.Name == "DeleteJob" || p.Name == "ViewApplicants" ||
                    p.Name == "ViewJobs" || p.Name == "UpdateProfile"))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = company.Id,
                        PermissionId = p.Id
                    });
                }

                // Applicant permissions
                foreach (var p in permissions.Where(p =>
                    p.Name == "ApplyJob" || p.Name == "SaveJob" ||
                    p.Name == "ViewJobs" || p.Name == "ViewCompanies" ||
                    p.Name == "ViewApplications" || p.Name == "UpdateProfile"))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = applicant.Id,
                        PermissionId = p.Id
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
