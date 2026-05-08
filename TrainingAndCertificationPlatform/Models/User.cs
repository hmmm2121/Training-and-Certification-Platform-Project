using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

[Index("Email", Name = "UQ__Users__A9D10534668305D3", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string Role { get; set; } = null!;

    [StringLength(256)]
    public string PasswordHash { get; set; } = null!;

    [InverseProperty("Instructor")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("Trainee")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("Instructor")]
    public virtual ICollection<InstructorAvailability> InstructorAvailabilities { get; set; } = new List<InstructorAvailability>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Trainee")]
    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();
}
