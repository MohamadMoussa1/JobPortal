using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.ApplicantDto;
public class ApplicantDto
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Skills { get; set; }
    public string? Image { get; set; }
    public string ExperienceLevel { get; set; }
}
