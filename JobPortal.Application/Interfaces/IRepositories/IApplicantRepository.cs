using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IRepositories;

public interface IApplicantRepository
{
    Task<Applicant?> GetByUserIdAsync(Guid userId);
    void Update(Applicant applicant);
    Task SaveChangesAsync();
}