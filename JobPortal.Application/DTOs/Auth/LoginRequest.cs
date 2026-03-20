using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
