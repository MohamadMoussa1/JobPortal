using JobPortal.Application.DTOs.JobApplicationDto;
using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortal.API.Controllers;

[ApiController]
[Route("api")]

public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _service;

    public JobApplicationsController(IJobApplicationService service)
    {
        _service = service;
    }
    [Authorize(Roles = "Applicant")]
    [HttpPost("jobs/apply")]
    public async Task<IActionResult> Apply([FromForm] ApplyJobDto dto)
    {
        // Get userId from JWT
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _service.ApplyAsync(userId, dto);

        return Ok(new { message = "Application submitted successfully" });
    }
    [Authorize(Roles = "Applicant")]
    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _service.GetMyApplicationsAsync(userId);

        return Ok(result);
    }
    [Authorize(Roles = "Company")]
    [HttpPatch("company/update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicationStatusDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _service.UpdateStatusAsync(userId, id, dto);

        return Ok(new { message = "Application status updated successfully" });
    }
    [Authorize(Roles = "Company")]
    [HttpGet("company/{jobId}/applications")]
    public async Task<IActionResult> GetApplications(Guid jobId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _service.GetJobApplicationsAsync(userId, jobId);

        return Ok(result);
    }
}
