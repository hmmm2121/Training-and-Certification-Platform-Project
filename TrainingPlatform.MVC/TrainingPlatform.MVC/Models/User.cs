using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Role { get; set; } = null!;

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<TraineeCertification> Certifications { get; set; } = new List<TraineeCertification>();
    }
}