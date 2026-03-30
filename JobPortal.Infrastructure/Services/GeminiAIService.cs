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
    public async Task<CVEnhancementResult> EnhanceCVAsync(string cvText, string jobDescription)
    {
        var jsonTemplate = """
        {
          "atsScore": <int 0-100>,
          "originalCvIssues": [
          "Future dates detected in experience section",
          "Missing professional summary",
          "Non-standard section names"
            ],
        "fixesApplied": [
          "Corrected all dates to MM/YYYY format",
          "Added keyword-rich professional summary",
          "Standardized all section names"
            ],
          "personalInfo": {
            "fullName": "...",
            "jobTitle": "...",
            "email": "...",
            "phone": "...",
            "linkedin": "...",
            "github": "...",
            "location": "..."
          },
          "sections": [
            {
              "sectionName": "Summary",
              "type": "text",
              "content": "..."
            },
            {
              "sectionName": "Experience",
              "type": "list",
              "items": [
                {
                  "title": "Job Title",
                  "subtitle": "Company Name",
                  "duration": "MM/YYYY - MM/YYYY",
                  "bullets": ["...", "...", "..."]
                },
                {
                  "title": "Job Title",
                  "subtitle": "Company Name",
                  "duration": "MM/YYYY - MM/YYYY",
                  "bullets": ["...", "...", "..."]
                }
              ]
            },
            {
              "sectionName": "Education",
              "type": "list",
              "items": [
                {
                  "title": "Degree",
                  "subtitle": "University Name",
                  "duration": "MM/YYYY - MM/YYYY",
                  "bullets": []
                }
              ]
            },
            {
              "sectionName": "Skills",
              "type": "tags",
              "tags": ["...", "...", "..."]
            },
            {
              "sectionName": "Projects",
              "type": "list",
              "items": [
                {
                  "title": "Project Name — Tech Stack",
                  "subtitle": "Personal Project / Company Name",
                  "duration": "",
                  "bullets": ["...", "...", "..."]
                },
                {
                  "title": "Project Name — Tech Stack",
                  "subtitle": "Personal Project / Company Name",
                  "duration": "",
                  "bullets": ["...", "...", "..."]
                },
                {
                  "title": "Project Name — Tech Stack",
                  "subtitle": "Personal Project / Company Name",
                  "duration": "",
                  "bullets": ["...", "...", "..."]
                }
              ]
            },
            {
              "sectionName": "Languages",
              "type": "tags",
              "tags": ["...", "..."]
            }
          ]
        }
        """;
        var today = DateTime.Now.ToString("MM/yyyy");

        var prompt = $"""
        You are a professional resume writer and ATS specialist.
        Today's date is {today}.

        Your job is to enhance the given CV based on the provided job description.
        You must improve the presentation of real content only — never fabricate, exaggerate, or invent anything.

        ═══════════════════════════════════════
        CONTENT ENHANCEMENT RULES
        ═══════════════════════════════════════
        Summary:
        - Write a concise 3-4 sentence professional summary
        - Reflect who the candidate actually is based on their real experience
        - Tailor it to the job description using real skills and experience from the CV
        - Do not add skills or experience that are not in the original CV

        Experience:
        - Rewrite bullet points using: Action Verb + Task + Result
        - Keep all original metrics exactly as they are (do not change numbers)
        - Expand vague bullet points into clear, specific statements
        - Each experience must have at least 5 bullet points
        - Only use information that exists in the original CV

        Projects:
        - Each project must have at least 3 bullet points
        - Clearly state the tech stack, what was built, and what problem it solves
        - Only use information that exists in the original CV

        Skills:
        - Group skills by category (Backend, Frontend, Database, DevOps, Tools, Soft Skills)
        - Add only keywords from the job description that are genuinely supported by the CV content
        - Remove skills that are clearly irrelevant or too basic

        ═══════════════════════════════════════
        ATS OPTIMIZATION RULES
        ═══════════════════════════════════════
        - Only flag dates as future if they are strictly after {today}
        - Standardize all dates to MM/YYYY format
        - Use standard section names: Summary, Experience, Education, Skills, Projects, Certifications, Languages
        - Enforce reverse chronological order for Experience, Education, and Projects
        - Integrate relevant keywords from the job description naturally into the content
        - Every bullet point must start with a strong action verb
        - Spell out acronyms at least once

        ═══════════════════════════════════════
        ATS SCORE RULES
        ═══════════════════════════════════════
        Calculate the atsScore honestly based on the ENHANCED CV using these weights:
        - Keyword match with job description: 30%
        - Bullet point quality (action verbs + results): 20%
        - Section structure and naming: 15%
        - Skills relevance and completeness: 10%
        - Professional summary quality: 10%
        - Date format consistency: 10%
        - Contact information completeness: 5%

        The score must reflect the true state of the enhanced CV.
        Do not inflate — if critical skills from the job description are genuinely missing, lower the score accordingly.

        ═══════════════════════════════════════
        HONESTY RULES — NEVER VIOLATE
        ═══════════════════════════════════════
        - Do not invent any experience, project, skill, or metric
        - Do not change existing numbers or metrics
        - Do not add keywords that are not supported by actual experience in the CV
        - Only improve how real content is presented — never what the content is

        ═══════════════════════════════════════
        COMPLETENESS RULES — NEVER OMIT
        ═══════════════════════════════════════
        - The JSON structure below is an EXAMPLE format only — not a limit
        - Include every project, experience, and education entry from the original CV
        - The number of items in each array must exactly match the original CV

        ═══════════════════════════════════════
        OUTPUT RULES
        ═══════════════════════════════════════
        - originalCvIssues: list specific issues found in the original CV
        - fixesApplied: list specific improvements made in the enhanced version
        - Do not use quotation marks inside string values
        - Return JSON only — no markdown, no explanation

        JSON structure (EXAMPLE ONLY — item count must reflect actual CV content):
        {jsonTemplate}

        Job Description:
        {jobDescription}

        Original CV Content:
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
                temperature = 0.4,
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

            var result = JsonSerializer.Deserialize<CVEnhancementResult>(text!, new JsonSerializerOptions
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
