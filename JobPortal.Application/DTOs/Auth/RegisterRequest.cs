using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!; // "Applicant" or "Company"
}
