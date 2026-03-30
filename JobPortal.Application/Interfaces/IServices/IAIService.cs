using JobPortal.Application.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface IAIService
{
    Task<ResumeAnalysisResult> AnalyzeResumeAsync(string cvText);
    Task<CVEnhancementResult> EnhanceCVAsync(string cvText, string jobDescription);
}
