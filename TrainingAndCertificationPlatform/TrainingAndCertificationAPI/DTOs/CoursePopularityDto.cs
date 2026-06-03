namespace TrainingAndCertificationAPI.DTOs
{
    public class CoursePopularityDto
    {
        public string Course { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public int TotalEnrollments { get; set; }
        public int AvailableCapacity { get; set; }
    }
}
