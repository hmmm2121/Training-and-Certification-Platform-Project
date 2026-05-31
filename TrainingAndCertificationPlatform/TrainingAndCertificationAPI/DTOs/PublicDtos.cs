namespace TrainingAndCertificationAPI.DTOs
{
    public class CertificationVerificationDto
    {
        public bool Found { get; set; }
        public string? TraineeName { get; set; }
        public string? TrackName { get; set; }
        public string? Status { get; set; }
        public DateTime? IssuedAt { get; set; }
        public List<string> CompletedCourses { get; set; } = new();
    }
}