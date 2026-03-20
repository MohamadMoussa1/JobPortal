using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SavedJob
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ApplicantId { get; set; }

    [Required]
    public Guid JobId { get; set; }

    [ForeignKey("ApplicantId")]
    public Applicant Applicant { get; set; } = default!;

    [ForeignKey("JobId")]
    public Job Job { get; set; } = default!;
}
