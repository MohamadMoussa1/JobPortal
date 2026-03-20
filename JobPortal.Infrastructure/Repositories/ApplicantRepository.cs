using JobPortal.Domain.Entities;
using JobPortal.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobPortal.Application.Interfaces.IRepositories;
namespace JobPortal.Infrastructure.Repositories;

public class ApplicantRepository : IApplicantRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Applicant?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Applicants
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }
    public void Update(Applicant applicant)
    {
        _context.Applicants.Update(applicant);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
