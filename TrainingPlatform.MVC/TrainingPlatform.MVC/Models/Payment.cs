using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingPlatform.MVC.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 99999.99)]
        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        public bool IsOverdue { get; set; } = false;
    }
}