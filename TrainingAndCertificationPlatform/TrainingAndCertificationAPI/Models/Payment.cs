using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int EnrollmentId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaidAt { get; set; }

    public DateOnly DueDate { get; set; }

    public bool IsOverdue { get; set; }

    [ForeignKey("EnrollmentId")]
    [InverseProperty("Payments")]
    public virtual Enrollment Enrollment { get; set; } = null!;
}
