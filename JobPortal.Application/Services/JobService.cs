using JobPortal.Application.DTOs.JobDto;
using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Application.Interfaces.IServices;
using JobPortal.Domain.Entities;
using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICompanyRepository _companyRepository;
    public JobService(IJobRepository jobRepository, ICompanyRepository companyRepository)
    {
        _jobRepository = jobRepository;
        _companyRepository = companyRepository;
    }

    public async Task CreateAsync(Guid userId, CreateJobDto dto)
    {
        // ✅ Validation (Business rules)
        if (dto.SalaryMin > dto.SalaryMax)
            throw new Exception("SalaryMin cannot be greater than SalaryMax");
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new Exception("Company profile not found");

        // ✅ Map DTO → Entity
        var job = new Job
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            Type = Enum.Parse<JobType>(dto.Type, true),
            RequiredExperience = dto.RequiredExperience != null ? Enum.Parse<ExperienceLevel>(dto.RequiredExperience, true) : (ExperienceLevel?)null,
            CreatedAt = DateTime.UtcNow,
            IsAvailable = true
        };

        // ✅ Save
        await _jobRepository.AddAsync(job);
        await _jobRepository.SaveChangesAsync();
    }
    //Get All Jobs related to this company
    public async Task<IEnumerable<JobDto>> GetCompanyJobsAsync(Guid userId)
    {
        // ✅ Get company from user
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company not found");

        // ✅ Get jobs
        var jobs = await _jobRepository.GetByCompanyIdAsync(company.Id);

        // ✅ Map to DTO
        var result = jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            Location = j.Location,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            Type = j.Type.ToString(),
            IsAvailable = j.IsAvailable,
            RequiredExperience = j.RequiredExperience.ToString(),
            CreatedAt = j.CreatedAt
        });

        return result;
    }
    //update specific job
    public async Task UpdateAsync(Guid userId, Guid jobId, UpdateJobDto dto)
    {
        // ✅ Get company
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company not found");

        // ✅ Get job
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null)
            throw new Exception("Job not found");

        // 🔐 SECURITY CHECK (CRITICAL)
        if (job.CompanyId != company.Id)
            throw new UnauthorizedAccessException("You cannot update this job");

        // ✅ Partial Update
        if (dto.Title != null)
            job.Title = dto.Title;

        if (dto.Description != null)
            job.Description = dto.Description;

        if (dto.Location != null)
            job.Location = dto.Location;

        if (dto.SalaryMin.HasValue)
            job.SalaryMin = dto.SalaryMin.Value;

        if (dto.SalaryMax.HasValue)
            job.SalaryMax = dto.SalaryMax.Value;

        // ✅ Validate after update
        if (job.SalaryMin > job.SalaryMax)
            throw new Exception("SalaryMin cannot be greater than SalaryMax");

        if (!string.IsNullOrWhiteSpace(dto.Type))
            job.Type = Enum.Parse<JobType>(dto.Type, true);

        if (!string.IsNullOrWhiteSpace(dto.RequiredExperience))
            job.RequiredExperience = Enum.Parse<ExperienceLevel>(dto.RequiredExperience, true);

        if (dto.IsAvailable.HasValue)
            job.IsAvailable = dto.IsAvailable.Value;

        await _jobRepository.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid userId, Guid jobId)
    {
        // ✅ Get company
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new Exception("Company not found");

        // ✅ Get job
        var job = await _jobRepository.GetByIdAsync(jobId);
        if (job == null)
            throw new Exception("Job not found");

        // 🔐 Ownership check
        if (job.CompanyId != company.Id)
            throw new UnauthorizedAccessException("You cannot delete this job");

        // ✅ Delete
        await _jobRepository.DeleteAsync(job);
        await _jobRepository.SaveChangesAsync();
    }
    public async Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetAllAsync(JobQueryParameters query)
    {
        var (jobs, totalCount) = await _jobRepository.GetAllAsync(query);

        // Map to DTO
        var result = jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            Location = j.Location,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            Type = j.Type.ToString(),
            IsAvailable = j.IsAvailable,
            RequiredExperience = j.RequiredExperience.ToString(),
            CreatedAt = j.CreatedAt
        });

        return (result, totalCount);
    }

}
