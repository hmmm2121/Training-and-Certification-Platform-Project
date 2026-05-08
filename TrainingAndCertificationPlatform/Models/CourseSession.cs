using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class CourseSession
{
    [Key]
    public int SessionId { get; set; }

    public int CourseId { get; set; }

    public int InstructorId { get; set; }

    public int RoomId { get; set; }

    public DateOnly SessionDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int MaxCapacity { get; set; }

    [StringLength(100)]
    public string Status { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("CourseSessions")]
    public virtual Course Course { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [ForeignKey("InstructorId")]
    [InverseProperty("CourseSessions")]
    public virtual User Instructor { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("CourseSessions")]
    public virtual Room Room { get; set; } = null!;
}
