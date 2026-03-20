using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JobPortal.Domain.Entities;

using JobPortal.Domain.Enums;

[Table("JobApplications")]
public class JobApplication
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid JobId { get; set; }

    [ForeignKey("JobId")]
    public Job Job { get; set; } = default!;

    [Required]
    public Guid ApplicantId { get; set; }

    [ForeignKey("ApplicantId")]
    public Applicant Applicant { get; set; } = default!;

    // Enum for status
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    // ✅ New: CV file path or URL
    [Required]
    [MaxLength(255)]
    public string Cv { get; set; } = default!;

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}