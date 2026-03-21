using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobDto;

public class UpdateJobDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }

    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }

    public string? Type { get; set; }
    public string? RequiredExperience { get; set; }

    public bool? IsAvailable { get; set; }
}