using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly TrainAndCertContext _context;

        public AccountController(TrainAndCertContext context)
        {
            _context = context;
        }

        // below are the actions for trainee registration
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email is already taken
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

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

        // below are the actions for instructor registration done ONLY by the training coordinator
        // GET: /Account/CreateInstructor
        [HttpGet]
        [Authorize(Roles = "TrainingCoordinator")]
        public IActionResult CreateInstructor()
        {
            return View();
        }

        // POST: /Account/CreateInstructor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> CreateInstructor(CreateInstructorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email is already taken
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

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


        // below are the actions for login/logout
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // action for access denied page
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}