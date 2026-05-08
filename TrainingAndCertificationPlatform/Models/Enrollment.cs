using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Enrollment
{
    [Key]
    public int EnrollmentId { get; set; }

    public int TraineeId { get; set; }

    public int SessionId { get; set; }

    [StringLength(100)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? EnrolledAt { get; set; }

    [InverseProperty("Enrollment")]
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    [InverseProperty("Enrollment")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("SessionId")]
    [InverseProperty("Enrollments")]
    public virtual CourseSession Session { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("Enrollments")]
    public virtual User Trainee { get; set; } = null!;
}
