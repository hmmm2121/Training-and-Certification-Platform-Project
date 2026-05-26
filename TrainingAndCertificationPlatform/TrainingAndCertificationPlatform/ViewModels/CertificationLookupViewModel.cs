namespace TrainingAndCertificationPlatform.ViewModels
{
    public class CertificationLookupViewModel
    {
        public string? TraineeId { get; set; }
        public string? CertificateRef { get; set; }
        public string? TraineeName { get; set; }
        public string? TrackName { get; set; }
        public string? CertificationStatus { get; set; }
        public DateTime? IssuedAt { get; set; }
        public List<string> CompletedCourses { get; set; } = new List<string>();
        public bool Found { get; set; } = false;
        public string? ErrorMessage { get; set; }
    }
}