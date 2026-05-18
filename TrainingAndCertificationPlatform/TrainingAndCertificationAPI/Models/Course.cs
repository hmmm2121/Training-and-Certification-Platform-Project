using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(100)]
    public string? Description { get; set; }

    public int DurationHours { get; set; }

    public int Capacity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Fee { get; set; }

    public int SubjectId { get; set; }

    public int? PrerequisiteCourseId { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("PrerequisiteCourse")]
    public virtual ICollection<Course> InversePrerequisiteCourse { get; set; } = new List<Course>();

    [ForeignKey("PrerequisiteCourseId")]
    [InverseProperty("InversePrerequisiteCourse")]
    public virtual Course? PrerequisiteCourse { get; set; }

    [ForeignKey("SubjectId")]
    [InverseProperty("Courses")]
    public virtual Subject Subject { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<TrackCourse> TrackCourses { get; set; } = new List<TrackCourse>();
}
