using JobPortal.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Infrastructure.Services;

public class FileService : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        var folderPath = Path.Combine("wwwroot", folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/{folderName}/{fileName}";
    }
}