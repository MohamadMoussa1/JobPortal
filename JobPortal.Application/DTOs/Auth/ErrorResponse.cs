using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.Auth;

public class ErrorResponse
{
    public string Message { get; set; } = default!;
    public object? Details { get; set; }

    public ErrorResponse(string message, object? details = null)
    {
        Message = message;
        Details = details;
    }
}
