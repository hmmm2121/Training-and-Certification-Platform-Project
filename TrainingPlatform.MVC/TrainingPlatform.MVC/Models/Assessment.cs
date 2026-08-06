using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class Assessment
    {
        public int AssessmentId { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Result { get; set; } = null!;

        [StringLength(100)]
        public string? Notes { get; set; }

        public DateTime? RecordedAt { get; set; }
    }
}