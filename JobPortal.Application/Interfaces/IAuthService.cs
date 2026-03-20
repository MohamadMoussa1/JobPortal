using JobPortal.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
}