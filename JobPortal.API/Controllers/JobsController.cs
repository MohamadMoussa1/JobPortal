using JobPortal.Application.DTOs.JobDto;
using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortal.API.Controllers;


[ApiController]
[Route("api")]
public class CompanyJobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public CompanyJobsController(IJobService jobService)
    {
        _jobService = jobService;
    }
    [Authorize(Roles = "Company")]
    [HttpPost("company/createjobs")]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        // ✅ Extract CompanyId from JWT
        var companyId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _jobService.CreateAsync(companyId, dto);

        return Ok(new { message = "Job created successfully" });
    }
    [Authorize(Roles = "Company")]
    [HttpGet("company/Alljobs")]
    public async Task<IActionResult> GetMyJobs()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var jobs = await _jobService.GetCompanyJobsAsync(userId);

        return Ok(jobs);
    }
    [Authorize(Roles = "Company")]
    [HttpPatch("company/update/{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateJobDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _jobService.UpdateAsync(userId, id, dto);

        return Ok(new { message = "Job updated successfully" });
    }
    [Authorize(Roles = "Company")]
    [HttpDelete("company/delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _jobService.DeleteAsync(userId, id);

        return Ok(new { message = "Job deleted successfully" });
    }
    [Authorize(Roles = "Applicant")]
    [HttpGet("applicant/jobs")]
    public async Task<IActionResult> GetAll([FromQuery] JobQueryParameters query)
    {
        var (jobs, totalCount) = await _jobService.GetAllAsync(query);

        return Ok(new
        {
            totalCount,
            page = query.Page,
            pageSize = query.PageSize,
            jobs
        });
    }
}