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
    [Authorize(Roles = "TrainingCoordinator")]
    public class TrackCoursesController : Controller
    {
        private readonly TrainAndCertContext _context;

        public TrackCoursesController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: TrackCourses
        public async Task<IActionResult> Index()
        {
            var trainAndCertContext = _context.TrackCourses.Include(t => t.Course).Include(t => t.Track);
            return View(await trainAndCertContext.ToListAsync());
        }

        // GET: TrackCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackCourse = await _context.TrackCourses
                .Include(t => t.Course)
                .Include(t => t.Track)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trackCourse == null)
            {
                return NotFound();
            }

            return View(trackCourse);
        }

        // GET: TrackCourses/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
            ViewData["TrackId"] = new SelectList(_context.CertificationTracks, "TrackId", "Name");
            return View();
        }

        // POST: TrackCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrackId,CourseId")] TrackCourse trackCourse)
        {
            // Validate that no duplicates exist
            if (_context.TrackCourses.Any(tc => tc.TrackId == trackCourse.TrackId && tc.CourseId == trackCourse.CourseId))
            {
                ModelState.AddModelError("", "This course is already associated with the selected track.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(trackCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", trackCourse.CourseId);
            ViewData["TrackId"] = new SelectList(_context.CertificationTracks, "TrackId", "Name", trackCourse.TrackId);
            return View(trackCourse);
        }

        // GET: TrackCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackCourse = await _context.TrackCourses.FindAsync(id);
            if (trackCourse == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", trackCourse.CourseId);
            ViewData["TrackId"] = new SelectList(_context.CertificationTracks, "TrackId", "Name", trackCourse.TrackId);
            return View(trackCourse);
        }

        // POST: TrackCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrackId,CourseId")] TrackCourse trackCourse)
        {
            if (id != trackCourse.Id)
            {
                return NotFound();
            }

            // Validate that no duplicates exist (excluding the current record)
            if (_context.TrackCourses.Any(tc => tc.TrackId == trackCourse.TrackId && tc.CourseId == trackCourse.CourseId && tc.Id != trackCourse.Id))
            {
                ModelState.AddModelError("", "This course is already associated with the selected track.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trackCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackCourseExists(trackCourse.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", trackCourse.CourseId);
            ViewData["TrackId"] = new SelectList(_context.CertificationTracks, "TrackId", "Name", trackCourse.TrackId);
            return View(trackCourse);
        }

        // GET: TrackCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackCourse = await _context.TrackCourses
                .Include(t => t.Course)
                .Include(t => t.Track)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trackCourse == null)
            {
                return NotFound();
            }

            return View(trackCourse);
        }

        // POST: TrackCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trackCourse = await _context.TrackCourses.FindAsync(id);
            if (trackCourse != null)
            {
                _context.TrackCourses.Remove(trackCourse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackCourseExists(int id)
        {
            return _context.TrackCourses.Any(e => e.Id == id);
        }
    }
}
