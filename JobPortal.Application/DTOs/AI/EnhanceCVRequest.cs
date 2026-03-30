using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.AI
{
    public class EnhanceCVRequest
    {
        public IFormFile File { get; set; } = null!;
        public string JobDescription { get; set; } = string.Empty;
    }
}
