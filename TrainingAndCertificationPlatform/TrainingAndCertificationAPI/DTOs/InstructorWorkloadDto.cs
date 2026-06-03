namespace TrainingAndCertificationAPI.DTOs
{
    public class InstructorWorkloadDto
    {
        public string Instructor { get; set; } = null!;
        public int SessionsTaught { get; set; }
        public int TotalTraineesTaught { get; set; }
    }
}
