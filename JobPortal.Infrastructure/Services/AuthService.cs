using JobPortal.Application.DTOs.Auth;
using JobPortal.Application.Interfaces;
using JobPortal.Domain.Entities;
using JobPortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace JobPortal.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ApplicationDbContext context, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Email already exists");

        var allowedRoles = new[] { "Applicant", "Company" };
        if (!allowedRoles.Contains(request.Role))
            throw new Exception("Invalid role selection");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
        if (role == null) throw new Exception("Role not found");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id
        };

        // Assign role-specific entity
        if (request.Role == "Applicant")
        {
            user.Applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = request.Name
            };
        }
        else if (request.Role == "Company")
        {
            user.Company = new Company
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = request.Name
            };
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Fetch permissions
        var permissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        var token = _jwtService.GenerateToken(user, new List<string> { role.Name }, permissions);

        // Set HTTP-only cookie
        _httpContextAccessor.HttpContext!.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(3)
        });

        return new AuthResponse
        {
            Email = user.Email,
            Roles = new List<string> { role.Name }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        var permissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == user.RoleId)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        var token = _jwtService.GenerateToken(user, new List<string> { user.Role.Name }, permissions);

        _httpContextAccessor.HttpContext!.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(3)
        });

        return new AuthResponse
        {
            Email = user.Email,
            Roles = new List<string> { user.Role.Name }
        };
    }

    public Task LogoutAsync()
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("jwt");
        return Task.CompletedTask;
    }
}
