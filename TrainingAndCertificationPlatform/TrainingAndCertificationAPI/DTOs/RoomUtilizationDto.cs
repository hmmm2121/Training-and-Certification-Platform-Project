namespace TrainingAndCertificationAPI.DTOs
{
    public class RoomUtilizationDto
    {
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public int TotalSessionsHosted { get; set; }
    }
}
