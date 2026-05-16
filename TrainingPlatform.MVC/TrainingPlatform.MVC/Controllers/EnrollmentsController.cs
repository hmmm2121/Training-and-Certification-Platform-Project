using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingPlatform.MVC.Data;
using TrainingPlatform.MVC.Models;

namespace TrainingPlatform.MVC.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly AppDbContext _context;

        public EnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // INDEX - Show all enrollments for the logged in trainee
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Instructor)
                .Include(e => e.Trainee)
                .ToListAsync();

            return View(enrollments);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Instructor)
                .Include(e => e.Trainee)
                .Include(e => e.Payments)
                .Include(e => e.Assessments)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            ViewData["SessionId"] = new SelectList(
                await _context.CourseSessions
                    .Include(s => s.Course)
                    .Select(s => new {
                        s.SessionId,
                        Display = s.Course.Title + " - " + s.SessionDate
                    }).ToListAsync(),
                "SessionId", "Display");

            ViewData["TraineeId"] = new SelectList(
                await _context.Users
                    .Where(u => u.Role == "Trainee")
                    .ToListAsync(),
                "UserId", "FullName");

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TraineeId, int SessionId)
        {
            // Get the session with course info
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.SessionId == SessionId);

            if (session == null) return NotFound();

            // Check 1: Capacity limit
            var currentCount = await _context.Enrollments
                .CountAsync(e => e.SessionId == SessionId && e.Status != "Dropped");

            if (currentCount >= session.MaxCapacity)
            {
                ModelState.AddModelError("", "This session is full. No available spots remaining.");
                await PopulateDropdowns();
                return View();
            }

            // Check 2: Already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == TraineeId && e.SessionId == SessionId);

            if (alreadyEnrolled)
            {
                ModelState.AddModelError("", "You are already enrolled in this session.");
                await PopulateDropdowns();
                return View();
            }

            // Check 3: Prerequisite
            if (session.Course.PrerequisiteCourseId != null)
            {
                var hasPassed = await _context.Enrollments
                    .Include(e => e.Session)
                    .Include(e => e.Assessments)
                    .AnyAsync(e => e.TraineeId == TraineeId
                        && e.Session.CourseId == session.Course.PrerequisiteCourseId
                        && e.Assessments.Any(a => a.Result == "Pass"));

                if (!hasPassed)
                {
                    ModelState.AddModelError("", "You have not completed the prerequisite course for this session.");
                    await PopulateDropdowns();
                    return View();
                }
            }

            // All checks passed - create enrollment
            var enrollment = new Enrollment
            {
                TraineeId = TraineeId,
                SessionId = SessionId,
                Status = "Enrolled",
                EnrolledAt = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successfully enrolled in the course!";
            return RedirectToAction(nameof(Index));
        }

        // UPDATE STATUS
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            // Valid status transitions
            var validTransitions = new Dictionary<string, List<string>>
            {
                { "Enrolled",   new List<string> { "Confirmed", "Dropped" } },
                { "Confirmed",  new List<string> { "Attending", "Dropped" } },
                { "Attending",  new List<string> { "Completed", "Dropped" } },
                { "Completed",  new List<string>() },
                { "Dropped",    new List<string>() }
            };

            if (!validTransitions[enrollment.Status].Contains(newStatus))
            {
                ModelState.AddModelError("", $"Cannot transition from {enrollment.Status} to {newStatus}.");
                return View(enrollment);
            }

            enrollment.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Enrollment status updated to {newStatus}.";
            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Enrollment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate dropdowns
        private async Task PopulateDropdowns()
        {
            ViewData["SessionId"] = new SelectList(
                await _context.CourseSessions
                    .Include(s => s.Course)
                    .Select(s => new {
                        s.SessionId,
                        Display = s.Course.Title + " - " + s.SessionDate
                    }).ToListAsync(),
                "SessionId", "Display");

            ViewData["TraineeId"] = new SelectList(
                await _context.Users
                    .Where(u => u.Role == "Trainee")
                    .ToListAsync(),
                "UserId", "FullName");
        }
    }
}