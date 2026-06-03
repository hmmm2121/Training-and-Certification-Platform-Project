using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    public class PublicLookupController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PublicLookupController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(new CertificationLookupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CertificationLookupViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TraineeId) ||
                string.IsNullOrWhiteSpace(model.CertificateRef))
            {
                model.ErrorMessage = "Please enter both Trainee ID and Certificate Reference Number.";
                return View(model);
            }

            if (!int.TryParse(model.TraineeId, out int traineeId))
            {
                model.ErrorMessage = "Invalid Trainee ID format.";
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                // get the API base URL from appsettings.json
                var apiBase = _configuration["ApiSettings:BaseUrl"];

                var url = $"{apiBase}/api/public/certifications/verify?traineeId={traineeId}&certificateRef={Uri.EscapeDataString(model.CertificateRef)}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error contacting the certification service. Please try again.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<CertificationVerificationResult>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null || !result.Found)
                {
                    model.ErrorMessage = "No certification found with the provided details.";
                    model.Found = false;
                    return View(model);
                }

                model.Found = true;
                model.TraineeName = result.TraineeName;
                model.TrackName = result.TrackName;
                model.CertificationStatus = result.Status;
                model.IssuedAt = result.IssuedAt;
                model.CompletedCourses = result.CompletedCourses;
            }
            catch (HttpRequestException)
            {
                model.ErrorMessage = "Could not reach the certification service. Please try again later.";
            }

            return View(model);
        }
    }

    public class CertificationVerificationResult
    {
        public bool Found { get; set; }
        public string? TraineeName { get; set; }
        public string? TrackName { get; set; }
        public string? Status { get; set; }
        public DateTime? IssuedAt { get; set; }
        public List<string> CompletedCourses { get; set; } = new();
    }
}