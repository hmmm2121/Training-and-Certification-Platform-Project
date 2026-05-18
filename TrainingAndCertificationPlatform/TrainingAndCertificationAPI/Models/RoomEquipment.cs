using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

[Table("RoomEquipment")]
public partial class RoomEquipment
{
    [Key]
    public int EquipmentId { get; set; }

    public int RoomId { get; set; }

    [StringLength(100)]
    public string EquipmentName { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("RoomEquipments")]
    public virtual Room Room { get; set; } = null!;
}
