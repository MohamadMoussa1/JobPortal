using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IRepositories;

public interface IJobApplicationRepository
{
    Task<bool> ExistsAsync(Guid jobId, Guid applicantId);
    Task AddAsync(JobApplication application);
    Task SaveChangesAsync();
    Task<List<JobApplication>> GetByApplicantIdAsync(Guid applicantId);
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<List<JobApplication>> GetByJobIdAsync(Guid jobId);

}