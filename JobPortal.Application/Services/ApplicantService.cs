using JobPortal.Application.DTOs.ApplicantDto;
using JobPortal.Application.Interfaces.IRepositories;
using JobPortal.Application.Interfaces.IServices;
using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services;

public class ApplicantService : IApplicantService
{
    private readonly IApplicantRepository _repo;
    private readonly IFileService _fileService;

    public ApplicantService(IApplicantRepository repo, IFileService fileService)
    {
        _repo = repo;
        _fileService = fileService;
    }

    public async Task<ApplicantDto> GetMyProfileAsync(Guid userId)
    {
        var applicant = await _repo.GetByUserIdAsync(userId);

        if (applicant == null)
            throw new Exception("Applicant profile not found");

        return new ApplicantDto
        {
            Name = applicant.Name,
            Location = applicant.Location,
            Phone = applicant.Phone,
            Website = applicant.Website,
            Skills = applicant.Skills,
            Image = applicant.Image,
            ExperienceLevel = applicant.ExperienceLevel.HasValue ? applicant.ExperienceLevel.Value.ToString() : null
        };
    }
    public async Task UpdateMyProfileAsync(Guid userId, UpdateApplicantDto dto)
    {
        var applicant = await _repo.GetByUserIdAsync(userId);

        if (applicant == null)
            throw new Exception("Applicant profile not found");

        // ✅ Partial Update Logic

        if (!string.IsNullOrWhiteSpace(dto.Name))
            applicant.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Location))
            applicant.Location = dto.Location;

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            applicant.Phone = dto.Phone;

        if (!string.IsNullOrWhiteSpace(dto.Website))
            applicant.Website = dto.Website;

        if (!string.IsNullOrWhiteSpace(dto.Skills))
            applicant.Skills = dto.Skills;

        if (dto.Image != null)
        {
            var allowedTypes = new[] { ".jpg", ".png", ".jpeg" };

            var extension = Path.GetExtension(dto.Image.FileName).ToLower();

            if (!allowedTypes.Contains(extension))
                throw new Exception("Invalid file type");

            //to delete the old image if this Applicant already has an image
            if (!string.IsNullOrEmpty(applicant.Image))
            {
                var oldPath = Path.Combine("wwwroot", applicant.Image.TrimStart('/'));

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }
            var imagePath = await _fileService.SaveFileAsync(dto.Image, "images");
            applicant.Image = imagePath;
        }
        if (!string.IsNullOrWhiteSpace(dto.ExperienceLevel))
            applicant.ExperienceLevel = Enum.Parse<ExperienceLevel>(dto.ExperienceLevel, true);

        _repo.Update(applicant);
        await _repo.SaveChangesAsync();
    }
}
