using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Subject
{
    [Key]
    public int SubjectId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Subject")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("Subject")]
    public virtual ICollection<InstructorSubject> InstructorSubjects { get; set; } = new List<InstructorSubject>();
}
