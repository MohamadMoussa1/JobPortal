using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Company
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = default!;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = default!;

    [MaxLength(150)]
    public string? Location { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    // ✅ New: Logo (optional)
    [MaxLength(255)]
    public string? Logo { get; set; }

    // ✅ New: Phone (optional)
    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    // ✅ New: Description (optional)
    [MaxLength(2000)]
    public string? Description { get; set; }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
