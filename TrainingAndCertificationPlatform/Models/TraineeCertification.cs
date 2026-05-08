using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class TraineeCertification
{
    [Key]
    public int CertificationId { get; set; }

    public int TraineeId { get; set; }

    public int TrackId { get; set; }

    [StringLength(100)]
    public string Status { get; set; } = null!;

    [StringLength(100)]
    public string? CertificateRef { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? IssuedAt { get; set; }

    [ForeignKey("TrackId")]
    [InverseProperty("TraineeCertifications")]
    public virtual CertificationTrack Track { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("TraineeCertifications")]
    public virtual User Trainee { get; set; } = null!;
}
