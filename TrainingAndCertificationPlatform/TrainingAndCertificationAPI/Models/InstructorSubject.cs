using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class InstructorSubject
{
    [Key]
    public int Id { get; set; }

    public int InstructorId { get; set; }

    public int SubjectId { get; set; }

    [ForeignKey("InstructorId")]
    [InverseProperty("InstructorSubjects")]
    public virtual User Instructor { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("InstructorSubjects")]
    public virtual Subject Subject { get; set; } = null!;
}
