using Microsoft.AspNetCore.Mvc;

namespace TrainingAndCertificationReports.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiAuthService _authService;

        public AccountController(ApiAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.LoginAsync(model.Email, model.Password);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password for API.");
                return View(model);
            }

            return RedirectToAction("EnrollmentStats", "Reports");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Login");
        }
    }
}