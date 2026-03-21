using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.JobDto;

public class JobQueryParameters
{
    public int Page { get; set; } = 1;      // default page 1
    public int PageSize { get; set; } = 10; // default 10 per page

    // Optional filters
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Type { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}
