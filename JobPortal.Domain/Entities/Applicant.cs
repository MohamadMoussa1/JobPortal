using JobPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace JobPortal.Domain.Entities;

public class Applicant
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = default!;

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = default!;

    [MaxLength(150)]
    public string? Location { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    
    [MaxLength(500)]
    public string? Skills { get; set; }

    
    [MaxLength(255)]
    public string? Image { get; set; }

    // Applicant experience level
    public ExperienceLevel? ExperienceLevel { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
}
