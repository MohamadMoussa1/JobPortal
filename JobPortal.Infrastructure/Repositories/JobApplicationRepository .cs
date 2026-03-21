using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobPortal.Infrastructure.Persistence;
namespace JobPortal.Infrastructure.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public JobApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid jobId, Guid applicantId)
    {
        return await _context.Applications
            .AnyAsync(x => x.JobId == jobId && x.ApplicantId == applicantId);
    }

    public async Task AddAsync(JobApplication application)
    {
        await _context.Applications.AddAsync(application);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<List<JobApplication>> GetByApplicantIdAsync(Guid applicantId)
    {
        return await _context.Applications
            .Include(x => x.Job) // IMPORTANT (to get JobTitle)
            .Where(x => x.ApplicantId == applicantId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();
    }
    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _context.Applications
            .Include(x => x.Job) // IMPORTANT (we need Job.CompanyId)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<List<JobApplication>> GetByJobIdAsync(Guid jobId)
    {
        return await _context.Applications
            .Include(x => x.Applicant)   // to get applicant info
            .Include(x => x.Job)         // for security check
            .Where(x => x.JobId == jobId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();
    }
}
