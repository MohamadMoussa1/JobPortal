using JobPortal.Application.DTOs.JobApplicationDto;
using JobPortal.Application.DTOs.JobDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface IJobService
{
    Task CreateAsync(Guid companyId, CreateJobDto dto);
    Task<IEnumerable<JobDto>> GetCompanyJobsAsync(Guid userId);
    Task UpdateAsync(Guid userId, Guid jobId, UpdateJobDto dto);
    Task DeleteAsync(Guid userId, Guid jobId);
    // IJobService.cs
    Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetAllAsync(JobQueryParameters query);
    
}
