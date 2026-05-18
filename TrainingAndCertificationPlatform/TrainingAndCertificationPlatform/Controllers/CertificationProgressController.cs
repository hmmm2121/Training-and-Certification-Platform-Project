using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.ViewModels;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize(Roles = "Trainee,TrainingCoordinator")]
    public class CertificationProgressController : Controller
    {
        private readonly TrainAndCertContext _context;

        public CertificationProgressController(TrainAndCertContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int trackId, int traineeId)
        {
            // If trainee, always force their own ID — they can never pick someone else
            if (User.IsInRole("Trainee"))
            {
                traineeId = int.Parse(
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
                );
            }
            else
            {
                // Coordinator sees a trainee dropdown
                ViewData["TraineeId"] = new SelectList(
                    _context.Users.Where(u => u.Role == "Trainee"),
                    "UserId", "FullName", traineeId
                );
            }

            // Everyone sees the track dropdown
            ViewData["TrackId"] = new SelectList(
                _context.CertificationTracks,
                "TrackId", "Name", trackId
            );

            // Don't query until both are selected
            if (traineeId == 0 || trackId == 0)
                return View(null);

            var trainee = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == traineeId);
            var track = await _context.CertificationTracks
                .FirstOrDefaultAsync(t => t.TrackId == trackId);

            if (trainee == null || track == null)
                return NotFound();

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
                .Where(a => a.Result == "Pass" && a.Enrollment.TraineeId == traineeId)
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