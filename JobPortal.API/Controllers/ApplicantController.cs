using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers;

using JobPortal.Application.DTOs.ApplicantDto;
using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize(Roles = "Applicant")]
[ApiController]
[Route("api/applicant")]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _service;

    public ApplicantController(IApplicantService service)
    {
        _service = service;
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
}