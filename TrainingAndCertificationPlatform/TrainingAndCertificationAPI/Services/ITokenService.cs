using Microsoft.AspNetCore.Identity;

namespace TrainingAndCertificationAPI.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(IdentityUser user);
    }
}
