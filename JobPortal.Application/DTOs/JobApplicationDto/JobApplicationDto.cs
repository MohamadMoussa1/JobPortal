using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobApplicationDto;

public class JobApplicationDto
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public string JobTitle { get; set; } = default!;
    public string CompanyName { get; set; } = default!;

    public string Cv { get; set; } = default!;

    public string Status { get; set; }

    public DateTime AppliedAt { get; set; }
}