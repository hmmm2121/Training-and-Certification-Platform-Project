using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    public class PublicLookupController : Controller
    {
        private readonly TrainAndCertContext _context;

        public PublicLookupController(TrainAndCertContext context)
        {
            _context = context;
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

            var certification = await _context.TraineeCertifications
                .Include(c => c.Trainee)
                .Include(c => c.Track)
                .FirstOrDefaultAsync(c =>
                    c.TraineeId == traineeId &&
                    c.CertificateRef == model.CertificateRef);

            if (certification == null)
            {
                model.ErrorMessage = "No certification found with the provided details.";
                model.Found = false;
                return View(model);
            }

            var completedCourses = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Where(a => a.Enrollment.TraineeId == traineeId
                    && a.Result == "Pass")
                .Select(a => a.Enrollment.Session.Course.Title)
                .Distinct()
                .ToListAsync();

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