using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime? CreatedAt { get; set; }
    }
}