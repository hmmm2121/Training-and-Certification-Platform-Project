using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; } = null!;

        [Required]
        public int Capacity { get; set; }

        public ICollection<RoomEquipment> Equipment { get; set; } = new List<RoomEquipment>();
        public ICollection<CourseSession> Sessions { get; set; } = new List<CourseSession>();
    }
}