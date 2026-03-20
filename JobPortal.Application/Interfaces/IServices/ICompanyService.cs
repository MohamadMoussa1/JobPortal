using JobPortal.Application.DTOs.CompanyDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface ICompanyService
{
    Task<CompanyDto> GetMyCompanyProfileAsync(Guid userId);
    Task UpdateMyProfileAsync(Guid userId, UpdateCompanyDto dto);
}
