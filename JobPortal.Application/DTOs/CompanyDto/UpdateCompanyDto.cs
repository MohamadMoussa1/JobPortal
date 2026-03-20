using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace JobPortal.Application.DTOs.CompanyDto;


public class UpdateCompanyDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }

    public IFormFile? Logo { get; set; } // file upload
}