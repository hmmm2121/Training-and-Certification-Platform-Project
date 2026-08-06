using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class CertificationTrack
    {
        public int TrackId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        public string? Description { get; set; }

        public ICollection<TrackCourse> TrackCourses { get; set; } = new List<TrackCourse>();
        public ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();
    }
}