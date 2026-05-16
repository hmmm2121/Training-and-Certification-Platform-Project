using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingPlatform.MVC.Data;
using TrainingPlatform.MVC.Models;

namespace TrainingPlatform.MVC.Controllers
{
    public class PublicLookupController : Controller
    {
        private readonly AppDbContext _context;

        public PublicLookupController(AppDbContext context)
        {
            _context = context;
        }

        // GET - Show the lookup form
        public IActionResult Index()
        {
            return View(new CertificationLookupViewModel());
        }

        // POST - Process the lookup
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

            // Parse trainee ID
            if (!int.TryParse(model.TraineeId, out int traineeId))
            {
                model.ErrorMessage = "Invalid Trainee ID format.";
                return View(model);
            }

            // Look up the certification
            var certification = await _context.TraineeCertifications
                .Include(c => c.Trainee)
                .Include(c => c.Track)
                    .ThenInclude(t => t.TrackCourses)
                        .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(c =>
                    c.TraineeId == traineeId &&
                    c.CertificateRef == model.CertificateRef);

            if (certification == null)
            {
                model.ErrorMessage = "No certification found with the provided details. Please check your Trainee ID and Certificate Reference Number.";
                model.Found = false;
                return View(model);
            }

            // Get completed courses for this trainee
            var completedCourses = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Assessments)
                .Where(e => e.TraineeId == traineeId
                    && e.Status == "Completed"
                    && e.Assessments.Any(a => a.Result == "Pass"))
                .Select(e => e.Session.Course.Title)
                .ToListAsync();

            // Populate the view model
            model.Found = true;
            model.TraineeName = certification.Trainee.FullName;
            model.TrackName = certification.Track.Name;
            model.CertificationStatus = certification.Status;
            model.IssuedAt = certification.IssuedAt;
            model.CompletedCourses = completedCourses;

            return View(model);
        }
    }
}