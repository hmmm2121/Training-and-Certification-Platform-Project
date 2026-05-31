using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly TrainAndCertContext _context;

        public EnrollmentsController(TrainAndCertContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Instructor)
                .Include(e => e.Trainee)
                .AsQueryable();

            // Trainees only see their own enrollments
            if (User.IsInRole("Trainee"))
            {
                var userId = int.Parse(User.FindFirst("AppUserId")!.Value);
                query = query.Where(e => e.TraineeId == userId);
            }

            return View(await query.ToListAsync());
        }

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

        [Authorize(Roles = "Trainee,TrainingCoordinator")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainee,TrainingCoordinator")]
        public async Task<IActionResult> Create(int TraineeId, int SessionId)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.SessionId == SessionId);

            if (session == null) return NotFound();

            // Check 1: Capacity
            var currentCount = await _context.Enrollments
                .CountAsync(e => e.SessionId == SessionId && e.Status != "Dropped");

            if (currentCount >= session.MaxCapacity)
            {
                ModelState.AddModelError("", "This session is full.");
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
                    ModelState.AddModelError("", "You have not completed the prerequisite course.");
                    await PopulateDropdowns();
                    return View();
                }
            }

            var enrollment = new Enrollment
            {
                TraineeId = TraineeId,
                SessionId = SessionId,
                Status = "Enrolled",
                EnrolledAt = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successfully enrolled!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "TrainingCoordinator")]
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
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            var validTransitions = new Dictionary<string, List<string>>
            {
                { "Enrolled",  new List<string> { "Confirmed", "Dropped" } },
                { "Confirmed", new List<string> { "Attending", "Dropped" } },
                { "Attending", new List<string> { "Completed", "Dropped" } },
                { "Completed", new List<string>() },
                { "Dropped",   new List<string>() }
            };

            if (!validTransitions[enrollment.Status].Contains(newStatus))
            {
                ModelState.AddModelError("", $"Cannot transition from {enrollment.Status} to {newStatus}.");
                return View(enrollment);
            }

            enrollment.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Status updated to {newStatus}.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "TrainingCoordinator")]
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Assessments)
                .Include(e => e.Payments)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment != null)
            {
                _context.Assessments.RemoveRange(enrollment.Assessments);
                _context.Payments.RemoveRange(enrollment.Payments);

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Enrollment and related records deleted.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            await PopulateDropdowns();
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId) return NotFound();

            var existing = await _context.Enrollments.FindAsync(id);
            if (existing == null) return NotFound();

            // Check capacity if session changed
            if (existing.SessionId != enrollment.SessionId)
            {
                var session = await _context.CourseSessions
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(s => s.SessionId == enrollment.SessionId);

                if (session == null) return NotFound();

                var currentCount = await _context.Enrollments
                    .CountAsync(e => e.SessionId == enrollment.SessionId && e.Status != "Dropped" && e.EnrollmentId != id);

                if (currentCount >= session.MaxCapacity)
                {
                    ModelState.AddModelError("", "This session is full.");
                    await PopulateDropdowns();
                    return View(enrollment);
                }
            }

            existing.TraineeId = enrollment.TraineeId;
            existing.SessionId = enrollment.SessionId;
            existing.Status = enrollment.Status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment updated successfully.";
            return RedirectToAction(nameof(Index));
        }

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