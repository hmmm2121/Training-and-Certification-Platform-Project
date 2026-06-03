using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingAndCertificationAPI.DTOs;
using TrainingAndCertificationAPI.Services;

namespace TrainingAndCertificationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager,ITokenService tokenService,IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            var token = await _tokenService.CreateTokenAsync(user);
            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"]!);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Email = user.Email!,
                Roles = roles
            };
        }
    }
}