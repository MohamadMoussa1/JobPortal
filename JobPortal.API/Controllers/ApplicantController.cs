using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers;

using JobPortal.Application.DTOs.AI;
using JobPortal.Application.DTOs.ApplicantDto;
using JobPortal.Application.Interfaces.IServices;
using JobPortal.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize(Roles = "Applicant")]
[ApiController]
[Route("api/applicant")]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _service;
    private readonly ResumeAnalyzerService _resumeAnalyzerService;
    private readonly CVEnhancementService _cvEnhancementService;
    private readonly CoverLetterService _coverLetterService;
    public ApplicantController(IApplicantService service, ResumeAnalyzerService resumeAnalyzerService, CVEnhancementService cvEnhancementService, CoverLetterService coverLetterService)
    {
        _service = service;
        _resumeAnalyzerService = resumeAnalyzerService;
        _cvEnhancementService = cvEnhancementService;
        _coverLetterService = coverLetterService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        // 1. Extract UserId from JWT
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        // 2. Call service
        var result = await _service.GetMyProfileAsync(Guid.Parse(userId));

        // 3. Return response
        return Ok(result);
    }
    [Authorize(Roles = "Applicant")]
    [HttpPatch("Updateme")]
    public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateApplicantDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _service.UpdateMyProfileAsync(Guid.Parse(userId), dto);

        return Ok("Profile updated successfully");
    }


    [HttpPost("cv/analyze")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> AnalyzeResume(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Please upload a CV file." });

        // Validate file type
        var allowedExtensions = new[] { ".pdf", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only PDF and DOCX files are allowed." });

        // Save to a temp location
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

        try
        {
            await using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = await _resumeAnalyzerService.AnalyzeAsync(tempPath);
            return Ok(result);
        }
        finally
        {
            // Always delete the temp file after analysis
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);
        }
    }



    [HttpPost("cv/enhance")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> EnhanceCV([FromForm] EnhanceCVRequest request)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { message = "Please upload a CV file." });

        var allowedExtensions = new[] { ".pdf", ".docx" };
        var extension = Path.GetExtension(request.File.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only PDF and DOCX files are allowed." });

        if (string.IsNullOrWhiteSpace(request.JobDescription))
            return BadRequest(new { message = "Job description is required." });

        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

        try
        {
            await using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var result = await _cvEnhancementService.EnhanceAsync(tempPath, request.JobDescription);
            return Ok(result);
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);
        }
    }


    [HttpPost("cover-letter/generate")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> GenerateCoverLetter([FromForm] GenerateCoverLetterRequest request)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { message = "Please upload a CV file." });

        var allowedExtensions = new[] { ".pdf", ".docx" };
        var extension = Path.GetExtension(request.File.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only PDF and DOCX files are allowed." });

        if (string.IsNullOrWhiteSpace(request.JobTitle))
            return BadRequest(new { message = "Job title is required." });

        if (string.IsNullOrWhiteSpace(request.CompanyName))
            return BadRequest(new { message = "Company name is required." });

        if (string.IsNullOrWhiteSpace(request.JobDescription))
            return BadRequest(new { message = "Job description is required." });

        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

        try
        {
            await using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var result = await _coverLetterService.GenerateAsync(
                tempPath,
                request.JobTitle,
                request.CompanyName,
                request.JobDescription,
                request.HrName);

            return Ok(result);
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);
        }
    }
}