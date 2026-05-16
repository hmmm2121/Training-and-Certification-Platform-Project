using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class TraineeCertification
    {
        public int CertificationId { get; set; }

        public int TraineeId { get; set; }
        public User Trainee { get; set; } = null!;

        public int TrackId { get; set; }
        public CertificationTrack Track { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Status { get; set; } = null!;

        [StringLength(100)]
        public string? CertificateRef { get; set; }

        public DateTime? IssuedAt { get; set; }
    }
}