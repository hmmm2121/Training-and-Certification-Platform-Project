using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TrainAndCertContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TrainAndCertContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ─── LOGIN ───────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var appUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            var identityUser = await _userManager.FindByEmailAsync(model.Email);

            if (appUser != null && identityUser != null)
            {
                var existingClaims = await _userManager.GetClaimsAsync(identityUser);
                var existingAppUserIdClaim = existingClaims
                    .FirstOrDefault(c => c.Type == "AppUserId");

                if (existingAppUserIdClaim != null)
                    await _userManager.RemoveClaimAsync(identityUser, existingAppUserIdClaim);

                await _userManager.AddClaimAsync(identityUser,
                    new Claim("AppUserId", appUser.UserId.ToString()));

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }

        // ─── LOGOUT ──────────────────────────────────────────────────────

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ─── TRAINEE REGISTER ────────────────────────────────────────────

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Create Identity user
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            // Assign Trainee role
            await _userManager.AddToRoleAsync(identityUser, "Trainee");

            // Create matching record in your Users table
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Role = "Trainee",
                PasswordHash = model.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Welcome, {user.FullName}! Your account has been created.";
            return RedirectToAction("Login");
        }

        // ─── CREATE INSTRUCTOR (Coordinator only) ────────────────────────

        [HttpGet]
        [Authorize(Roles = "TrainingCoordinator")]
        public IActionResult CreateInstructor() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> CreateInstructor(CreateInstructorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Create Identity user
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            // Assign Instructor role
            await _userManager.AddToRoleAsync(identityUser, "Instructor");

            // Create matching record in your Users table
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Role = "Instructor",
                PasswordHash = model.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Instructor account for {user.FullName} has been created.";
            return RedirectToAction("Index", "Home");
        }

        // ─── ACCESS DENIED ───────────────────────────────────────────────

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}