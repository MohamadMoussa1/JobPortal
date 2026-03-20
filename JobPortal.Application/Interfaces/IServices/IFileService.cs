using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces.IServices;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
}
