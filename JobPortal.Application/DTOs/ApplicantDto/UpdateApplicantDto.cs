using JobPortal.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.ApplicantDto;

public class UpdateApplicantDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Skills { get; set; }
    public IFormFile? Image { get; set; }
    public string? ExperienceLevel { get; set; }
}
