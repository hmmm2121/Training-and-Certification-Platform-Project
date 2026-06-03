public class TraineePaymentViewModel
{
    public string TraineeName { get; set; } = "";
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public int OverdueCount { get; set; }
}