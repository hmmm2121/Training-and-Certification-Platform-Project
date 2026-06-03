namespace TrainingAndCertificationAPI.DTOs
{
    public class EnrollmentStatDto
    {
        public string Course { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public int TotalEnrollments { get; set; }
        public int Completed { get; set; }
        public int Dropped { get; set; }
    }
}
