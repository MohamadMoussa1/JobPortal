using JobPortal.Application.DTOs.JobDto;
using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<IActionResult> GetMyJobs([FromQuery] JobQueryParameters query)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _jobService.GetCompanyJobsAsync(userId, query);

        return Ok(result);
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
        var result = await _jobService.GetAllAsync(query);

        return Ok(result);
    }
}