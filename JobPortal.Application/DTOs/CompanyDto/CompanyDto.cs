using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.CompanyDto;

public class CompanyDto
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? Logo { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
}