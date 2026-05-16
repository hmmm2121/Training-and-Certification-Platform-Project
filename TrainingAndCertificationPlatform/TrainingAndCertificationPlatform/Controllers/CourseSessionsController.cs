using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize(Roles = "Instructor,TrainingCoordinator")]
    public class CourseSessionsController : Controller
    {
        private readonly TrainAndCertContext _context;

        public CourseSessionsController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: CourseSessions
        public async Task<IActionResult> Index()
        {
            var query = _context.CourseSessions
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .AsQueryable();

            if (User.IsInRole("Instructor"))
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                query = query.Where(cs => cs.InstructorId == userId);
            }

            return View(await query.ToListAsync());
        }

        // GET: CourseSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSession = await _context.CourseSessions
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (courseSession == null)
            {
                return NotFound();
            }

            return View(courseSession);
        }

        // GET: CourseSessions/Create
        [Authorize(Roles = "TrainingCoordinator")]
        public IActionResult Create()
        {
            // populates dropdowns
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
            ViewData["InstructorId"] =
                new SelectList(
                    _context.Users
                    .Where(u => u.Role == "Instructor"), "UserId", "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName");
            return View();
        }

        // POST: CourseSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Create([Bind("SessionId,CourseId,InstructorId,RoomId,SessionDate,StartTime,EndTime,MaxCapacity")] CourseSession courseSession)
        {
            // Validates that EndTime is after StartTime
            if (courseSession.EndTime <= courseSession.StartTime)
            {
                ModelState.AddModelError("EndTime", "End Time must be after Start Time.");
            }

            // Validates that SessionDate is not in the past
            if (courseSession.SessionDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("SessionDate", "Session Date cannot be in the past.");
            }

            // Validates that no instructor is double booked
            if (_context.CourseSessions.Any(cs =>
                cs.InstructorId == courseSession.InstructorId &&
                cs.SessionDate == courseSession.SessionDate &&
                courseSession.StartTime < cs.EndTime &&
                courseSession.EndTime > cs.StartTime))
            {
                ModelState.AddModelError("InstructorId", "This instructor is already booked for another session at the same time.");
            }

            // Validates that no room is double booked
            if (_context.CourseSessions.Any(cs =>
                cs.RoomId == courseSession.RoomId &&
                cs.SessionDate == courseSession.SessionDate &&
                courseSession.StartTime < cs.EndTime &&
                courseSession.EndTime > cs.StartTime))
            {
                ModelState.AddModelError("RoomId", "This room is already booked for another session at the same time.");
            }

            // Validates that the room capacity is sufficient for the course session
            var room = _context.Rooms.Find(courseSession.RoomId);
            if (room != null && courseSession.MaxCapacity > room.Capacity)
            {
                ModelState.AddModelError("MaxCapacity", $"Max Capacity cannot exceed room capacity of {room.Capacity}.");
            }

            // Sets session status based on current date and time, then saves if valid
            courseSession.Status = GetSessionStatus(courseSession);
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                courseSession.Status = GetSessionStatus(courseSession);
                _context.Add(courseSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // re-populates dropdowns if validation fails
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", courseSession.CourseId);
            ViewData["InstructorId"] =
                new SelectList(
                    _context.Users
                    .Where(u => u.Role == "Instructor"), "UserId", "FullName", courseSession.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", courseSession.RoomId);
            return View(courseSession);
        }

        // GET: CourseSessions/Edit/5
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSession = await _context.CourseSessions.FindAsync(id);
            if (courseSession == null)
            {
                return NotFound();
            }
            // populates dropdowns
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", courseSession.CourseId);
            ViewData["InstructorId"] =
                new SelectList(
                    _context.Users
                    .Where(u => u.Role == "Instructor"), "UserId", "FullName", courseSession.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", courseSession.RoomId);
            return View(courseSession);
        }

        // POST: CourseSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,CourseId,InstructorId,RoomId,SessionDate,StartTime,EndTime,MaxCapacity")] CourseSession courseSession)
        {
            // Validates that EndTime is after StartTime
            if (courseSession.EndTime <= courseSession.StartTime)
            {
                ModelState.AddModelError("EndTime", "End Time must be after Start Time.");
            }

            // Validates that SessionDate is not in the past
            if (courseSession.SessionDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("SessionDate", "Session Date cannot be in the past.");
            }

            // Validates that no instructor double-books themselves
            if (_context.CourseSessions.Any(cs =>
                cs.SessionId != courseSession.SessionId &&
                cs.InstructorId == courseSession.InstructorId &&
                cs.SessionDate == courseSession.SessionDate &&
                courseSession.StartTime < cs.EndTime &&
                courseSession.EndTime > cs.StartTime))
            {
                ModelState.AddModelError("InstructorId", "This instructor is already booked for another session at the same time.");
            }

            // Validates that no room double-books itself
            if (_context.CourseSessions.Any(cs =>
                cs.SessionId != courseSession.SessionId &&
                cs.RoomId == courseSession.RoomId &&
                cs.SessionDate == courseSession.SessionDate &&
                courseSession.StartTime < cs.EndTime &&
                courseSession.EndTime > cs.StartTime))
            {
                ModelState.AddModelError("RoomId", "This room is already booked for another session at the same time.");
            }

            // Validates that the room capacity is sufficient for the course session
            var room = _context.Rooms.Find(courseSession.RoomId);
            if (room != null && courseSession.MaxCapacity > room.Capacity)
            {
                ModelState.AddModelError("MaxCapacity", $"Max Capacity cannot exceed room capacity of {room.Capacity}.");
            }
            if (id != courseSession.SessionId)
            {
                return NotFound();
            }

            // Sets session status based on current date and time, then saves if valid
            courseSession.Status = GetSessionStatus(courseSession);
            ModelState.Remove("Status");
            if (ModelState.IsValid)
            {
                try
                {
                    courseSession.Status = GetSessionStatus(courseSession);
                    _context.Update(courseSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseSessionExists(courseSession.SessionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // re-populates dropdowns if validation fails
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", courseSession.CourseId);
            ViewData["InstructorId"] =
                new SelectList(
                    _context.Users
                    .Where(u => u.Role == "Instructor"), "UserId", "FullName", courseSession.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", courseSession.RoomId);
            return View(courseSession);
        }

        // GET: CourseSessions/Delete/5
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseSession = await _context.CourseSessions
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (courseSession == null)
            {
                return NotFound();
            }

            return View(courseSession);
        }

        // POST: CourseSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseSession = await _context.CourseSessions.FindAsync(id);
            if (courseSession != null)
            {
                _context.CourseSessions.Remove(courseSession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseSessionExists(int id)
        {
            return _context.CourseSessions.Any(e => e.SessionId == id);
        }

        // method to automatically set session to Scheduled, Ongoing, or Completed based on current date and time
        private string GetSessionStatus(CourseSession session)
        {
            var now = DateTime.Now;
            var start = session.SessionDate.ToDateTime(session.StartTime);
            var end = session.SessionDate.ToDateTime(session.EndTime);

            if (now < start)
            {
                return "Scheduled";
            }

            if (now >= start && now <= end)
            {
                return "Ongoing";
            }

            return "Completed";
        }
    }
}
