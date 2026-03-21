using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobDto;

public class CreateJobDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;

    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }

    public string Type { get; set; }
    public string? RequiredExperience { get; set; }
}