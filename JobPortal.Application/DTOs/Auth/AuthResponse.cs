using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.Auth;

public class AuthResponse
{
    
    public string Email { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
}
