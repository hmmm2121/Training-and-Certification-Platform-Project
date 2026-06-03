namespace TrainingAndCertificationAPI.DTOs
{
    public class TraineePaymentDto
    {
        public string TraineeName { get; set; } = null!;
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverdueCount { get; set; }
    }
}
