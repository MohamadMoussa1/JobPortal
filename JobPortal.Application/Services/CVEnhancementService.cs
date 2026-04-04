using JobPortal.Application.DTOs.AI;
using JobPortal.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services;

public class CVEnhancementService
{
    private readonly IAIService _aiService;
    private readonly ICVTextExtractor _extractor;

    public CVEnhancementService(IAIService aiService, ICVTextExtractor extractor)
    {
        _aiService = aiService;
        _extractor = extractor;
    }

    public async Task<CVEnhancementResult> EnhanceAsync(string cvFilePath, string jobDescription)
    {
        if (!File.Exists(cvFilePath))
            throw new FileNotFoundException("CV file not found.", cvFilePath);

        if (string.IsNullOrWhiteSpace(jobDescription))
            throw new ArgumentException("Job description is required.");

        var cvText = await _extractor.ExtractTextAsync(cvFilePath);

        if (string.IsNullOrWhiteSpace(cvText))
            throw new InvalidOperationException("Could not extract text from the CV file.");

        return await _aiService.EnhanceCVAsync(cvText, jobDescription);
    }
}
