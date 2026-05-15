using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    //[Authorize(Roles = "Trainee,Training Coordinator")]
    public class CertificationProgressController : Controller
    {
        private readonly TrainAndCertContext _context;

        public CertificationProgressController(TrainAndCertContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int traineeId, int trackId)
        {
            var trainee = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == traineeId);
            var track = await _context.CertificationTracks
                .FirstOrDefaultAsync(t => t.TrackId == trackId);

            if (trainee == null || track == null)
            {
                return NotFound();
            }

            // Required courses for this certification track
            var requiredCourses = await _context.TrackCourses
                .Where(tc => tc.TrackId == trackId)
                .Include(tc => tc.Course)
                .Select(tc => tc.Course.Title)
                .ToListAsync();

            // Courses the trainee passed
            var completedCourses = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Where(a =>
                    a.Result == "Pass" && a.Enrollment.TraineeId == traineeId)
                .Select(a => a.Enrollment.Session.Course.Title)
                .Distinct()
                .ToListAsync();

            // Courses still missing
            var missingCourses = requiredCourses
                .Except(completedCourses)
                .ToList();

            var viewModel = new CertificationProgressViewModel
            {
                TraineeId = traineeId,
                TraineeName = trainee.FullName,
                TrackId = trackId,
                TrackName = track.Name,
                RequiredCourses = requiredCourses,
                CompletedCourses = completedCourses,
                MissingCourses = missingCourses,
                IsEligible = !missingCourses.Any()
            };
            return View(viewModel);
        }
    }
}