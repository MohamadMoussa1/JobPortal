using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace JobPortal.Application.DTOs.AI;

public class CVEnhancementResult
{
    public int AtsScore { get; set; }
    public List<string> OriginalCvIssues { get; set; } = [];   
    public List<string> FixesApplied { get; set; } = [];
    public PersonalInfo PersonalInfo { get; set; } = new();
    public List<CVSection> Sections { get; set; } = [];
}
