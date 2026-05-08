using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class CertificationTrack
{
    [Key]
    public int TrackId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Description { get; set; }

    [InverseProperty("Track")]
    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();
}
