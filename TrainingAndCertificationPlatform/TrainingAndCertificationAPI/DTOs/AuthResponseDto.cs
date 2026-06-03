namespace TrainingAndCertificationAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = null!;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
