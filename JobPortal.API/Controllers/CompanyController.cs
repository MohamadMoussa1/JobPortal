using JobPortal.Application.DTOs.CompanyDto;
using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortal.API.Controllers;


[Authorize(Roles = "Company")]
[ApiController]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompanyController(ICompanyService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyCompanyProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var result = await _service.GetMyCompanyProfileAsync(Guid.Parse(userId));

        return Ok(result);
    }
    [Authorize(Roles = "Company")]
    [HttpPatch("Updateme")]
    public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateCompanyDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _service.UpdateMyProfileAsync(Guid.Parse(userId), dto);

        return Ok("Company profile updated successfully");
    }
}