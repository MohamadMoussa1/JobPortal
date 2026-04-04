using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class SectionScores
{
    public int ContactInfo { get; set; }        // %
    public int WorkExperience { get; set; }     // %
    public int Education { get; set; }          // %
    public int Skills { get; set; }             // %
    public int Achievements { get; set; }       // %
    public int Formatting { get; set; }         // %
    public int KeywordsRelevance { get; set; }  // %
}