using JobPortal.Application.DTOs.JobDto;
using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IRepositories;

public interface IJobRepository
{
    Task AddAsync(Job job);
    Task SaveChangesAsync();
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId);
    Task<Job?> GetByIdAsync(Guid id);
    Task DeleteAsync(Job job);
    Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllAsync(JobQueryParameters query);
    
}