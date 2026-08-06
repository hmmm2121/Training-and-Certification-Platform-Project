using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class CourseSession
    {
        public int SessionId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int InstructorId { get; set; }
        public User Instructor { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        [Required]
        public DateOnly SessionDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}