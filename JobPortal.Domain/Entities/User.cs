using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = default!;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = default!;

    [Required]
    public Guid RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; } = default!;

    public Applicant? Applicant { get; set; }

    public Company? Company { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}