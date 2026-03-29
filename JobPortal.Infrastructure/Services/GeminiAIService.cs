using JobPortal.Application.DTOs.AI;
using JobPortal.Application.Interfaces.IServices;
using JobPortal.Application.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace JobPortal.Infrastructure.Services;

public class GeminiAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;

    public GeminiAIService(HttpClient httpClient, IOptions<GeminiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<ResumeAnalysisResult> AnalyzeResumeAsync(string cvText)
    {
        var jsonTemplate = """
                {
                  "overallScore": <int 0-100>,
                  "sectionScores": {
                    "contactInfo": <int 0-100>,
                    "workExperience": <int 0-100>,
                    "education": <int 0-100>,
                    "skills": <int 0-100>,
                    "achievements": <int 0-100>,
                    "formatting": <int 0-100>,
                    "keywordsRelevance": <int 0-100>
                  },
                  "strengths": ["...", "..."],
                  "weaknesses": ["...", "..."],
                  "missingKeywords": ["...", "..."],
                  "suggestions": ["...", "..."],
                  "summary": "..."
                }
                """;

        var prompt = $"""
                You are an expert HR consultant and resume coach.
                Analyze the following CV/Resume and return a JSON object only with no markdown, no explanation.

                JSON structure:
                {jsonTemplate}

                Scoring guide for each section (0-100%):
                - contactInfo: Has name, email, phone, LinkedIn/GitHub?
                - workExperience: Relevant roles, descriptions, impact, duration?
                - education: Degree, institution, graduation year present?
                - skills: Relevant technical and soft skills listed?
                - achievements: Quantified results, awards, certifications?
                - formatting: Clean layout, consistent structure, readable?
                - keywordsRelevance: Industry-relevant keywords present?

                The overallScore should be a weighted average of the section scores.

                CV Content:
                {cvText}
                """;

        var requestBody = new
        {
            contents = new[]
            {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
            generationConfig = new
            {
                temperature = 0.3,
                responseMimeType = "application/json"
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            var result = JsonSerializer.Deserialize<ResumeAnalysisResult>(text!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? throw new InvalidOperationException("Failed to deserialize AI response.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse Gemini response: {ex.Message}");
        }
    }
}
