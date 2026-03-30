using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class CVSection
{
    public string SectionName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // text | list | tags
    public string? Content { get; set; }                      // for type: text
    public List<CVSectionItem>? Items { get; set; }           // for type: list
    public List<string>? Tags { get; set; }                   // for type: tags
}
