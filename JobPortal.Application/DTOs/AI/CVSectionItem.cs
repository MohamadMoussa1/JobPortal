using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI;

public class CVSectionItem
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<string> Bullets { get; set; } = [];
}
