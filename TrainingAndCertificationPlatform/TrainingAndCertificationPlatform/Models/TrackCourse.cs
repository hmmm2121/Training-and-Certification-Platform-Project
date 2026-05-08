using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class TrackCourse
{
    [Key]
    public int Id { get; set; }

    public int TrackId { get; set; }

    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("TrackCourses")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("TrackId")]
    [InverseProperty("TrackCourses")]
    public virtual CertificationTrack Track { get; set; } = null!;
}
