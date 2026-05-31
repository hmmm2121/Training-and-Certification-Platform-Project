using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Assessment
{
    [Key]
    public int AssessmentId { get; set; }

    [Display(Name = "Enrollment")]
    public int EnrollmentId { get; set; }

    [StringLength(100)]
    public string Result { get; set; } = null!;

    [StringLength(100)]
    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RecordedAt { get; set; }

    [ForeignKey("EnrollmentId")]
    [InverseProperty("Assessments")]
    [ValidateNever]
    public virtual Enrollment Enrollment { get; set; } = null!;
}
