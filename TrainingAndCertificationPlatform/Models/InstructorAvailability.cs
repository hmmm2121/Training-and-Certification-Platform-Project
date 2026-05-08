using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

[Table("InstructorAvailability")]
public partial class InstructorAvailability
{
    [Key]
    public int AvailabilityId { get; set; }

    public int InstructorId { get; set; }

    public DateOnly AvailableDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [ForeignKey("InstructorId")]
    [InverseProperty("InstructorAvailabilities")]
    public virtual User Instructor { get; set; } = null!;
}
