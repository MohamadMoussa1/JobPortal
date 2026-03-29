using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class ResumeAnalysisResult
{
    public int OverallScore { get; set; }              // 0–100
    public SectionScores SectionScores { get; set; } = new();
    public List<string> Strengths { get; set; } = [];
    public List<string> Weaknesses { get; set; } = [];
    public List<string> MissingKeywords { get; set; } = [];
    public List<string> Suggestions { get; set; } = [];
    public string Summary { get; set; } = string.Empty;
}
