using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class RoomEquipment
    {
        public int EquipmentId { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string EquipmentName { get; set; } = null!;
    }
}