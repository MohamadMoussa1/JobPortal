using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Domain.Enums
{
    // Job type: Full-time or Part-time
    public enum JobType
    {
        FullTime,
        PartTime,
        Internship,   // <-- new value added
        Contract      // <-- another new value
    }

    // Applicant experience level
    public enum ExperienceLevel
    {
        Student,
        Junior,
        Mid,
        Senior
    }

    // Application status
    public enum ApplicationStatus
    {
        Pending,
        Reviewing,
        Accepted,
        Rejected
    }
}
