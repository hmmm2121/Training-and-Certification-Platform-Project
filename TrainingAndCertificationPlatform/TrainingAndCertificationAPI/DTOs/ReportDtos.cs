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

    public class InstructorWorkloadDto
    {
        public string Instructor { get; set; } = null!;
        public int SessionsTaught { get; set; }
        public int TotalTraineesTaught { get; set; }
    }

    public class CertificationRateDto
    {
        public string Track { get; set; } = null!;
        public int TotalTrainees { get; set; }
        public int Eligible { get; set; }
        public double CompletionRatePercent { get; set; }
    }

    public class RevenueReportDto
    {
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverduePayments { get; set; }
    }

    public class CoursePopularityDto
    {
        public string Course { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public int TotalEnrollments { get; set; }
        public int AvailableCapacity { get; set; }
    }

    public class RoomUtilizationDto
    {
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public int TotalSessionsHosted { get; set; }
    }

    public class TraineePaymentDto
    {
        public string TraineeName { get; set; } = null!;
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverdueCount { get; set; }
    }
}