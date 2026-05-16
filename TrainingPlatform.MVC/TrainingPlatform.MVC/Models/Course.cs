using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingPlatform.MVC.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        public int DurationHours { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Fee { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int? PrerequisiteCourseId { get; set; }
        public Course? Prerequisite { get; set; }

        public ICollection<TrackCourse> TrackCourses { get; set; } = new List<TrackCourse>();
        public ICollection<CourseSession> Sessions { get; set; } = new List<CourseSession>();
    }
}