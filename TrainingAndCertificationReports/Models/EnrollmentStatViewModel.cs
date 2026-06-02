public class EnrollmentStatViewModel
{
    public string Course { get; set; } = "";
    public string Subject { get; set; } = "";
    public int TotalEnrollments { get; set; }
    public int Completed { get; set; }
    public int Dropped { get; set; }
}