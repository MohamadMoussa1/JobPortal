using JobPortal.Application.DTOs.AI;
using JobPortal.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Services
{
    public class CoverLetterService
    {
        private readonly IAIService _aiService;
        private readonly ICVTextExtractor _extractor;

        public CoverLetterService(IAIService aiService, ICVTextExtractor extractor)
        {
            _aiService = aiService;
            _extractor = extractor;
        }

        public async Task<CoverLetterResult> GenerateAsync(
            string cvFilePath,
            string jobTitle,
            string companyName,
            string jobDescription,
            string? hrName)
        {
            if (!File.Exists(cvFilePath))
                throw new FileNotFoundException("CV file not found.", cvFilePath);

            if (string.IsNullOrWhiteSpace(jobTitle))
                throw new ArgumentException("Job title is required.");

            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name is required.");

            if (string.IsNullOrWhiteSpace(jobDescription))
                throw new ArgumentException("Job description is required.");

            var cvText = await _extractor.ExtractTextAsync(cvFilePath);

            if (string.IsNullOrWhiteSpace(cvText))
                throw new InvalidOperationException("Could not extract text from the CV file.");

            return await _aiService.GenerateCoverLetterAsync(
                cvText,
                jobTitle,
                companyName,
                jobDescription,
                hrName);
        }
    }
}
