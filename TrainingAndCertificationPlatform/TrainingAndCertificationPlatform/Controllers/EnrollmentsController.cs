using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Hubs;
using TrainingAndCertificationPlatform.Models;
using TrainingAndCertificationPlatform.Services;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly TrainAndCertContext _context;
        private readonly IHubContext<EnrollmentHub> _hub;
        private readonly NotificationService _notificationService;

        public EnrollmentsController(TrainAndCertContext context, IHubContext<EnrollmentHub> hub, NotificationService notificationService)
        {
            _context = context;
            _hub = hub;
            _notificationService = notificationService;
        }

        // works out how many spots are left in a session and tells everyone
        // who is looking at that session's page so their counter updates live
        private async Task NotifySpotsChanged(int sessionId)
        {
            var session = await _context.CourseSessions.FindAsync(sessionId);
            if (session == null) return;

            // dropped enrollments don't take up a spot
            var taken = await _context.Enrollments
                .CountAsync(e => e.SessionId == sessionId && e.Status != "Dropped");

            var spotsLeft = session.MaxCapacity - taken;

            // send the session id too, so the sessions list page knows which row to update
            await _hub.Clients.Group($"session-{sessionId}")
                .SendAsync("SpotsUpdated", new { sessionId, spotsLeft });
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
            // trainee can only enroll themselves
            if (User.IsInRole("Trainee"))
            {
                TraineeId = int.Parse(User.FindFirst("AppUserId")!.Value);
            }

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

            // trainee enrolled so counter gets updated
            await NotifySpotsChanged(SessionId);

            // notification is sent to the trainee that lets them know that the enrollment was successful
            await _notificationService.NotifyEnrollmentConfirmed(TraineeId, session.Course.Title);

            // notidfy the instructor that someone enrolled in their session
            var trainee = await _context.Users.FindAsync(TraineeId);
            await _notificationService.NotifyInstructorNewEnrollment(
                session.InstructorId, trainee?.FullName ?? "A trainee", session.Course.Title);

            TempData["Success"] = "Successfully enrolled!";
            return RedirectToAction(nameof(Index));
        }

        // allowed next statuses for each current status
        private static readonly Dictionary<string, List<string>> ValidTransitions = new()
        {
            { "Enrolled",  new List<string> { "Confirmed", "Dropped" } },
            { "Confirmed", new List<string> { "Attending", "Dropped" } },
            { "Attending", new List<string> { "Completed", "Dropped" } },
            { "Completed", new List<string>() },
            { "Dropped",   new List<string>() }
        };

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

            ViewBag.AllowedStatuses = ValidTransitions.TryGetValue(enrollment.Status, out var next)
                ? next
                : new List<string>();

            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();

            var allowed = ValidTransitions.TryGetValue(enrollment.Status, out var next)
                ? next
                : new List<string>();

            if (!allowed.Contains(newStatus))
            {
                ModelState.AddModelError("", $"Cannot transition from {enrollment.Status} to {newStatus}.");
                ViewBag.AllowedStatuses = allowed;
                return View(enrollment);
            }

            enrollment.Status = newStatus;
            await _context.SaveChangesAsync();

            // dropping frees up a spot, so refresh the live counter
            if (newStatus == "Dropped")
            {
                await NotifySpotsChanged(enrollment.SessionId);
            }

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
                // remember the session before deleting so we can update its counter after
                var sessionId = enrollment.SessionId;

                _context.Assessments.RemoveRange(enrollment.Assessments);
                _context.Payments.RemoveRange(enrollment.Payments);

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                // deleting an enrollment frees up a spot, so refresh the live counter
                await NotifySpotsChanged(sessionId);
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
            // status is changed only through Update Status, so it is left untouched here

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