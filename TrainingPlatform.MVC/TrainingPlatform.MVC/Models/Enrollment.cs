using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        public int TraineeId { get; set; }
        public User Trainee { get; set; } = null!;

        public int SessionId { get; set; }
        public CourseSession Session { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Enrolled";

        public DateTime? EnrolledAt { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    }
}