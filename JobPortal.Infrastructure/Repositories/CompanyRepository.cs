using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Domain.Entities;
using JobPortal.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace JobPortal.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
    public void Update(Company company)
    {
            _context.Companies.Update(company);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}