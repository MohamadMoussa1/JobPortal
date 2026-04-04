using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class CoverLetterResult
{
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public CoverLetterCandidate Candidate { get; set; } = new();
    public List<CoverLetterParagraph> Paragraphs { get; set; } = [];
}

public class CoverLetterCandidate
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
}

public class CoverLetterParagraph
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
