using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IRepositories;

public interface ICompanyRepository
{
    Task<Company?> GetByUserIdAsync(Guid userId);
    void Update(Company company);
    Task SaveChangesAsync();
}
