using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class GenerateCoverLetterRequest
{
    public IFormFile File { get; set; } = null!;
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string? HrName { get; set; }
}
