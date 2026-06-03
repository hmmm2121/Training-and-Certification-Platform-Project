namespace TrainingAndCertificationAPI.DTOs
{
    public class CertificationRateDto
    {
        public string Track { get; set; } = null!;
        public int TotalTrainees { get; set; }
        public int Eligible { get; set; }
        public double CompletionRatePercent { get; set; }
    }
}
