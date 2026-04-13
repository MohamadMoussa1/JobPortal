using JobPortal.Application.DTOs.JobDto;
using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Domain.Entities;
using JobPortal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetByCompanyIdAsync(Guid companyId,JobQueryParameters query)
    {
        var jobsQuery = _context.Jobs
            .Where(j => j.CompanyId == companyId)
            .AsQueryable();

        // Title — case insensitive
        if (!string.IsNullOrEmpty(query.Title))
            jobsQuery = jobsQuery.Where(j =>
                j.Title.ToLower().Contains(query.Title.ToLower()));

        // CompanyName — case insensitive
        if (!string.IsNullOrEmpty(query.CompanyName))
            jobsQuery = jobsQuery.Where(j =>
                j.Company.Name.ToLower().Contains(query.CompanyName.ToLower()));

        // Description — case insensitive
        if (!string.IsNullOrEmpty(query.Description))
            jobsQuery = jobsQuery.Where(j =>
                j.Description.ToLower().Contains(query.Description.ToLower()));

        // Location — case insensitive
        if (!string.IsNullOrEmpty(query.Location))
            jobsQuery = jobsQuery.Where(j =>
                j.Location.ToLower().Contains(query.Location.ToLower()));

        // Type — case insensitive
        if (!string.IsNullOrWhiteSpace(query.Type))
            jobsQuery = jobsQuery.Where(j =>
                j.Type.ToString().ToLower() == query.Type.ToLower());

        // MinSalary — no change needed (decimal comparison)
        if (query.MinSalary.HasValue)
            jobsQuery = jobsQuery.Where(j =>
                j.SalaryMin >= query.MinSalary.Value);

        // MaxSalary — no change needed (decimal comparison)
        if (query.MaxSalary.HasValue)
            jobsQuery = jobsQuery.Where(j =>
                j.SalaryMax <= query.MaxSalary.Value);
        

        // Total count before pagination
        var totalCount = await jobsQuery.CountAsync();

        // Pagination
        var jobs = await jobsQuery
            .OrderByDescending(j => j.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (jobs, totalCount);
    }
    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
    }
    public async Task DeleteAsync(Job job)
    {
        _context.Jobs.Remove(job);
    }
    public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllAsync(JobQueryParameters query)
    {
        var jobsQuery = _context.Jobs.Include(x=> x.Company).AsQueryable();

        // Only available jobs
        jobsQuery = jobsQuery.Where(j => j.IsAvailable);

        // Optional filters
        // Title — case insensitive
        if (!string.IsNullOrEmpty(query.Title))
            jobsQuery = jobsQuery.Where(j =>
                j.Title.ToLower().Contains(query.Title.ToLower()));

        // CompanyName — case insensitive
        if (!string.IsNullOrEmpty(query.CompanyName))
            jobsQuery = jobsQuery.Where(j =>
                j.Company.Name.ToLower().Contains(query.CompanyName.ToLower()));

        // Description — case insensitive
        if (!string.IsNullOrEmpty(query.Description))
            jobsQuery = jobsQuery.Where(j =>
                j.Description.ToLower().Contains(query.Description.ToLower()));

        // Location — case insensitive
        if (!string.IsNullOrEmpty(query.Location))
            jobsQuery = jobsQuery.Where(j =>
                j.Location.ToLower().Contains(query.Location.ToLower()));

        // Type — case insensitive
        if (!string.IsNullOrWhiteSpace(query.Type))
            jobsQuery = jobsQuery.Where(j =>
                j.Type.ToString().ToLower() == query.Type.ToLower());

        // MinSalary — no change needed (decimal comparison)
        if (query.MinSalary.HasValue)
            jobsQuery = jobsQuery.Where(j =>
                j.SalaryMin >= query.MinSalary.Value);

        // MaxSalary — no change needed (decimal comparison)
        if (query.MaxSalary.HasValue)
            jobsQuery = jobsQuery.Where(j =>
                j.SalaryMax <= query.MaxSalary.Value);
        // Total count before pagination
        var totalCount = await jobsQuery.CountAsync();

        // Pagination
        var jobs = await jobsQuery
            .OrderByDescending(j => j.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (jobs, totalCount);
    }
}