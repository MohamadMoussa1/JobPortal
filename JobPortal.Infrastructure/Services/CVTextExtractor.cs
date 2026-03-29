using DocumentFormat.OpenXml.Packaging;
using JobPortal.Application.Interfaces.IServices;
using System.Text;
using UglyToad.PdfPig;

namespace JobPortal.Infrastructure.Services;

public class CVTextExtractor : ICVTextExtractor
{
    public Task<string> ExtractTextAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        var text = extension switch
        {
            ".pdf" => ExtractFromPdf(filePath),
            ".docx" => ExtractFromDocx(filePath),
            _ => throw new NotSupportedException($"File type '{extension}' is not supported.")
        };

        return Task.FromResult(text);
    }

    private string ExtractFromPdf(string filePath)
    {
        var sb = new StringBuilder();
        using var pdf = PdfDocument.Open(filePath);
        foreach (var page in pdf.GetPages())
            sb.AppendLine(page.Text);
        return sb.ToString();
    }

    private string ExtractFromDocx(string filePath)
    {
        var sb = new StringBuilder();
        using var doc = WordprocessingDocument.Open(filePath, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body is null) return string.Empty;
        foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
            sb.AppendLine(text.Text);
        return sb.ToString();
    }
}
