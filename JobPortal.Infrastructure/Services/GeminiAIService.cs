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
        var today = DateTime.Now.ToString("MM/yyyy");

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
        You are an expert HR consultant and resume coach with 10+ years of experience
        reviewing CVs for top companies.

        Today's date is {today}. Use this to correctly evaluate whether dates are in the past or future.

        Your task is to analyze the given CV and return an honest, accurate assessment.

        SCORING RULES — calculate each section score honestly:
        - contactInfo (0-100):
            * Has full name? (+25)
            * Has email? (+25)
            * Has phone? (+25)
            * Has LinkedIn or GitHub? (+25)

        - workExperience (0-100):
            * Relevant roles present? (+25)
            * Clear descriptions with responsibilities? (+25)
            * Quantified achievements and impact? (+25)
            * Valid past dates — only flag as future if strictly after {today}? (+25)

        - education (0-100):
            * Degree name present? (+34)
            * Institution name present? (+33)
            * Graduation year present? (+33)

        - skills (0-100):
            * Relevant technical skills listed? (+50)
            * Soft skills listed? (+25)
            * Skills grouped by category? (+25)

        - achievements (0-100):
            * Quantified results present (%, numbers)? (+40)
            * Awards or recognitions mentioned? (+30)
            * Certifications listed? (+30)

        - formatting (0-100):
            * Clean and consistent structure? (+34)
            * Standard section names? (+33)
            * Readable and well organized? (+33)

        - keywordsRelevance (0-100):
            * Industry-relevant technical keywords present? (+50)
            * Keywords naturally integrated into content? (+25)
            * No keyword stuffing? (+25)

        OVERALL SCORE:
        Calculate the overallScore as a weighted average:
        - workExperience: 25%
        - skills: 20%
        - keywordsRelevance: 15%
        - achievements: 15%
        - formatting: 10%
        - education: 10%
        - contactInfo: 5%

        HONESTY RULES:
        - The score must be real and honest — do not inflate or deflate
        - Only flag dates as future if they are strictly after {today}
        - Strengths must be specific to what is actually in the CV
        - Weaknesses must be real issues found in the CV
        - Missing keywords must be genuinely absent from the CV
        - Suggestions must be actionable and specific to this CV
        - Do not use quotation marks inside string values
        - Return JSON only with no markdown and no explanation

        JSON structure:
        {jsonTemplate}

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

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(requestBody)
        };

        _httpClient.DefaultRequestHeaders.Clear();

        var response = await _httpClient.SendAsync(requestMessage);

        // ← Add this instead of EnsureSuccessStatusCode
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Gemini API error: {response.StatusCode} - {errorBody}");
        }
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
    public async Task<CoverLetterResult> GenerateCoverLetterAsync(
    string cvText,
    string jobTitle,
    string companyName,
    string jobDescription,
    string? hrName)
    {
        var jsonTemplate = """
        {
          "jobTitle": "...",
          "companyName": "...",
          "date": "...",
          "candidate": {
            "fullName": "...",
            "email": "...",
            "phone": "...",
            "linkedIn": "..."
          },
          "paragraphs": [
            {
              "key": "greeting",
              "label": "Greeting",
              "content": "Dear Hiring Manager, or Dear [HR Name],"
            },
            {
              "key": "opening",
              "label": "Opening",
              "content": "Who you are + position applying for + company name"
            },
            {
              "key": "whyMe",
              "label": "Why Me",
              "content": "Most relevant real skills and achievements matching the job"
            },
            {
              "key": "whyCompany",
              "label": "Why This Company",
              "content": "Why specifically this company and this role"
            },
            {
              "key": "closing",
              "label": "Closing",
              "content": "Thank them + express desire for interview"
            },
            {
              "key": "signOff",
              "label": "Sign Off",
              "content": "Sincerely,"
            }
          ]
        }
        """;
        var today = DateTime.Now.ToString("MMMM yyyy");
        var greeting = string.IsNullOrWhiteSpace(hrName)
            ? "Dear Hiring Manager,"
            : $"Dear {hrName},";

        var prompt = $"""
        You are a professional cover letter writer with 10+ years of experience.
        You write honest, compelling, and professional cover letters based strictly on the candidate's real experience.

        Today's date is {today}.

        Your task is to write a formal professional cover letter decomposed into named paragraphs.

        RULES YOU MUST FOLLOW:
        - Extract candidate info (fullName, email, phone, linkedIn) from the CV
        - The greeting paragraph must be exactly: {greeting}
        - The date must be: {today}
        - The letter must have exactly these paragraphs in this order:
            * greeting   → {greeting}
            * opening    → who the candidate is + position + company name
            * whyMe      → most relevant real skills and achievements from CV that match the job
            * whyCompany → genuine interest in this specific company and role
            * closing    → thank them + express desire for interview
            * signOff    → Sincerely,
        - Each paragraph must be 3-5 sentences
        - Tone must be formal, professional, and confident
        - Tailor every paragraph specifically to the job description and company
        - Only use skills and achievements that actually exist in the CV
        - Keep real metrics from the CV exactly as they are (e.g. 20%, 25%)
        - Do not invent any experience, skill, or achievement
        - Do not use quotation marks inside paragraph content
        - Return JSON only with no markdown and no explanation

        JSON structure:
        {jsonTemplate}

        Position: {jobTitle}
        Company: {companyName}

        Job Description:
        {jobDescription}

        Candidate CV:
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
                temperature = 0.5,
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

            var result = JsonSerializer.Deserialize<CoverLetterResult>(text!, new JsonSerializerOptions
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
