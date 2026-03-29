using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface ICVTextExtractor
{
    Task<string> ExtractTextAsync(string filePath);
}
