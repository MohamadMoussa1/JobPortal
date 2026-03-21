using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobApplicationDto;

public class ApplyJobDto
{
    public Guid JobId { get; set; }

    public IFormFile Cv { get; set; } = default!;
}
