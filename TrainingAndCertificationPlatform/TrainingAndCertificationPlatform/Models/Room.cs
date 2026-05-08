using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class Room
{
    [Key]
    public int RoomId { get; set; }

    [StringLength(100)]
    public string RoomName { get; set; } = null!;

    public int Capacity { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("Room")]
    public virtual ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
}
