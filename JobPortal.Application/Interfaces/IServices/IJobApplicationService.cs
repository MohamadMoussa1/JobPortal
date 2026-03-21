using JobPortal.Application.DTOs.JobApplicationDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface IJobApplicationService
{
    Task ApplyAsync(Guid userId, ApplyJobDto dto);
    Task<List<JobApplicationDto>> GetMyApplicationsAsync(Guid userId);
    Task UpdateStatusAsync(Guid userId, Guid applicationId, UpdateApplicationStatusDto dto);
    Task<List<JobApplicationForCompanyDto>> GetJobApplicationsAsync(Guid userId, Guid jobId);
}
