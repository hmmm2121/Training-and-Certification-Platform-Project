namespace TrainingPlatform.MVC.Models
{
    public class TrackCourse
    {
        public int Id { get; set; }

        public int TrackId { get; set; }
        public CertificationTrack Track { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}