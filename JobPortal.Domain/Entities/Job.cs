using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Domain.Entities;

using JobPortal.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

public class Job
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = default!;

    [Required]
    [MaxLength(150)]
    public string Location { get; set; } = default!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal SalaryMin { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal SalaryMax { get; set; }

    // Enum for job type
    public JobType Type { get; set; } = JobType.FullTime;
    // Required experience for the job
    public ExperienceLevel? RequiredExperience { get; set; }

    // ✅ New: Available or not
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
