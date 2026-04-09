using JobPortal.Application.DTOs.JobApplicationDto;
using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Application.Interfaces.IServices;
using JobPortal.Domain.Entities;
using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IApplicantRepository _applicantRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IFileService _fileService;
    private readonly ICompanyRepository _companyRepository;

    public JobApplicationService(
        IApplicantRepository applicantRepository,
        IJobRepository jobRepository,
        IJobApplicationRepository applicationRepository,
        IFileService fileService,
        ICompanyRepository companyRepository)   
    {
        _applicantRepository = applicantRepository;
        _jobRepository = jobRepository;
        _applicationRepository = applicationRepository;
        _fileService = fileService;
        _companyRepository = companyRepository;
    }

    public async Task ApplyAsync(Guid userId, ApplyJobDto dto)
    {
        // 1. Get applicant
        var applicant = await _applicantRepository.GetByUserIdAsync(userId);
        if (applicant == null)
            throw new Exception("Applicant not found");

        // 2. Validate CV
        if (dto.Cv == null || dto.Cv.Length == 0)
            throw new Exception("CV is required");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(dto.Cv.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Invalid CV format");

        // 3. Check job exists
        var job = await _jobRepository.GetByIdAsync(dto.JobId);
        if (job == null)
            throw new Exception("Job not found");

        // 4. Prevent duplicate application
        var exists = await _applicationRepository
            .ExistsAsync(dto.JobId, applicant.Id);

        if (exists)
            throw new Exception("You already applied to this job");

        // 5. Save CV file
        var cvPath = await _fileService.SaveFileAsync(dto.Cv, "cvs");

        // 6. Create JobApplication entity
        var application = new JobApplication
        {
            JobId = dto.JobId,
            ApplicantId = applicant.Id,
            Cv = cvPath
        };

        // 7. Save to DB
        await _applicationRepository.AddAsync(application);
        await _applicationRepository.SaveChangesAsync();
    }
    public async Task<List<JobApplicationDto>> GetMyApplicationsAsync(Guid userId)
    {
        // 1. Get applicant
        var applicant = await _applicantRepository.GetByUserIdAsync(userId);

        if (applicant == null)
            throw new Exception("Applicant not found");

        // 2. Get applications
        var applications = await _applicationRepository
            .GetByApplicantIdAsync(applicant.Id);

        // 3. Map to DTO
        return applications.Select(x => new JobApplicationDto
        {
            Id = x.Id,
            JobId = x.JobId,
            JobTitle = x.Job.Title, // from Include
            CompanyName=x.Job.Company.Name,
            Cv = x.Cv,
            Status = x.Status.ToString()    ,
            AppliedAt = x.AppliedAt
        }).ToList();
    }
    public async Task UpdateStatusAsync(Guid userId, Guid applicationId, UpdateApplicationStatusDto dto)
    {
        // 1. Get company by logged-in user
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company not found");

        // 2. Get application
        var application = await _applicationRepository.GetByIdAsync(applicationId);

        if (application == null)
            throw new Exception("Application not found");

        // 3. SECURITY: Check job belongs to this company
        if (application.Job.CompanyId != company.Id)
            throw new Exception("Unauthorized");

        

        // 5. Update status
        application.Status =Enum.Parse<ApplicationStatus>(dto.Status, true);

        // 6. Save changes
        await _applicationRepository.SaveChangesAsync();
    }
    public async Task<List<JobApplicationForCompanyDto>> GetJobApplicationsAsync(Guid userId, Guid jobId)
    {
        // 1. Get company
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company not found");

        // 2. Get job
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null)
            throw new Exception("Job not found");

        // 3. SECURITY: Check ownership
        if (job.CompanyId != company.Id)
            throw new Exception("Unauthorized");

        // 4. Get applications
        var applications = await _applicationRepository.GetByJobIdAsync(jobId);

        // 5. Map to DTO
        return applications.Select(x => new JobApplicationForCompanyDto
        {
            Id = x.Id,
            ApplicantId = x.ApplicantId,
            ApplicantName = x.Applicant.Name, // adjust based on your model
            Image = x.Applicant.Image, // adjust based on your model
            Cv = x.Cv,
            Status = x.Status.ToString(),
            AppliedAt = x.AppliedAt
        }).ToList();
    }
}
