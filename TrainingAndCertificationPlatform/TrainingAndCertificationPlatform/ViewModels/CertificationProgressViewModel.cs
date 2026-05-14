namespace TrainingAndCertificationPlatform.ViewModels
{
    public class CertificationProgressViewModel
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = "";

        public int TrackId { get; set; }
        public string TrackName { get; set; } = "";

        public List<string> RequiredCourses { get; set; } = new();
        public List<string> CompletedCourses { get; set; } = new();
        public List<string> MissingCourses { get; set; } = new();

        public bool IsEligible { get; set; }
    }
}
