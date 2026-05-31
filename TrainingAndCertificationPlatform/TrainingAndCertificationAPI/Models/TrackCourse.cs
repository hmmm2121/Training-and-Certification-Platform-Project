using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class TrackCourse
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Track")]
    public int TrackId { get; set; }

    [Display(Name = "Course")]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("TrackCourses")]
    [ValidateNever]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("TrackId")]
    [InverseProperty("TrackCourses")]
    [ValidateNever]
    public virtual CertificationTrack Track { get; set; } = null!;
}
