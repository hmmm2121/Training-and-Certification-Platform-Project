namespace TrainingAndCertificationAPI.DTOs
{
    public class RevenueReportDto
    {
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverduePayments { get; set; }
    }
}
