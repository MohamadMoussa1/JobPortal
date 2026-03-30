using JobPortal.Application.DTOs.CompanyDto;
using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repo;
    private readonly IFileService _fileService;

    public CompanyService(ICompanyRepository repo, IFileService fileService)
    {
        _repo = repo;
        _fileService = fileService;
    }

    public async Task<CompanyDto> GetMyCompanyProfileAsync(Guid userId)
    {
        var company = await _repo.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company profile not found");

        return new CompanyDto
        {
            Name = company.Name,
            Location = company.Location,
            Website = company.Website,
            Logo = company.Logo,
            Phone = company.Phone,
            Description = company.Description
        };
    }
    public async Task UpdateMyProfileAsync(Guid userId, UpdateCompanyDto dto)
    {
        var company = await _repo.GetByUserIdAsync(userId);

        if (company == null)
            throw new Exception("Company profile not found");

        // 🔹 Partial update
        if (!string.IsNullOrWhiteSpace(dto.Name))
            company.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Location))
            company.Location = dto.Location;

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            company.Phone = dto.Phone;

        if (!string.IsNullOrWhiteSpace(dto.Website))
            company.Website = dto.Website;

        if (!string.IsNullOrWhiteSpace(dto.Description))
            company.Description = dto.Description;

        //  Handle Logo Upload
        if (dto.Logo != null)
        {
            var allowedTypes = new[] { ".jpg", ".png", ".jpeg" };

            var extension = Path.GetExtension(dto.Logo.FileName).ToLower();

            if (!allowedTypes.Contains(extension))
                throw new Exception("Invalid file type");

            // Delete old logo if exists
            if (!string.IsNullOrEmpty(company.Logo))
            {
                var oldPath = Path.Combine("wwwroot", company.Logo.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // Save new logo
            var logoPath = await _fileService.SaveFileAsync(dto.Logo, "logos");
            company.Logo = logoPath;
        }

        _repo.Update(company);
        await _repo.SaveChangesAsync();
    }

}