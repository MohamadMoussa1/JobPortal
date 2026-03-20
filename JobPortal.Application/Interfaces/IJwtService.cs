using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, List<string> roles, List<string> permissions);
}
