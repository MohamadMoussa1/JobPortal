using JobPortal.Application.DTOs.ApplicantDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface IApplicantService
{
    Task<ApplicantDto> GetMyProfileAsync(Guid userId);
    Task UpdateMyProfileAsync(Guid userId, UpdateApplicantDto dto);
}