using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobApplicationDto;

public class JobApplicationForCompanyDto
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }

    public string ApplicantName { get; set; } = default!;
    public string? Image { get; set; }
    public string Cv { get; set; } = default!;

    public string Status { get; set; }

    public DateTime AppliedAt { get; set; }
}
