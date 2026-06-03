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
    public class CoursesController : Controller
    {
        private readonly TrainAndCertContext _context;

        public CoursesController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: Courses
        // anyone can browse the course catalogue, even without signing in
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var trainAndCertContext = _context.Courses.Include(c => c.PrerequisiteCourse).Include(c => c.Subject);
            return View(await trainAndCertContext.ToListAsync());
        }

        // GET: Courses/Details/5
        // course details are public too so prospective trainees can read them
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.PrerequisiteCourse)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            var prerequisites = _context.Courses
                .Select(c => new SelectListItem
    {
                 Value = c.CourseId.ToString(),
                 Text = c.Title
                })
             .ToList();

                prerequisites.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "None"
                });

            ViewData["PrerequisiteCourseId"] =
                new SelectList(prerequisites, "Value", "Text");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Title,Description,DurationHours,Capacity,Fee,SubjectId,PrerequisiteCourseId")] Course course)
        {
            if (ModelState.IsValid)
            {
                // makes sure no duplicated title is created
                if (_context.Courses.Any(c => c.Title == course.Title))
                {
                    ModelState.AddModelError("Title", "A course with this title already exists.");
                }

                // Prevents illogical prerequisit dependency loop 
                if (course.PrerequisiteCourseId != null)
                {
                    var prerequisite = await _context.Courses.FindAsync(course.PrerequisiteCourseId);

                    if (prerequisite != null &&
                        prerequisite.PrerequisiteCourseId == course.CourseId)
                    {
                        ModelState.AddModelError(
                            "PrerequisiteCourseId",
                            "Circular prerequisites are not allowed."
                        );
                    }
                }

                // checks the validity again after checking the prerequisit loop
                if (ModelState.IsValid)
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // repopulates the dropdown list incase of error
            var prerequisites = _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.Title
                })
                .ToList();

            prerequisites.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "None"
            });

            ViewData["PrerequisiteCourseId"] =
                new SelectList(prerequisites, "Value", "Text", course.PrerequisiteCourseId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name", course.SubjectId);

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var prerequisites = _context.Courses
                .Where(c => c.CourseId != course.CourseId)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.Title
                })
                .ToList();

                prerequisites.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "None"
                });

            ViewData["PrerequisiteCourseId"] =
                new SelectList(prerequisites, "Value", "Text", course.PrerequisiteCourseId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name", course.SubjectId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Description,DurationHours,Capacity,Fee,SubjectId,PrerequisiteCourseId")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            // checks whether there's a course with the same title the user is trying to provide
            if (ModelState.IsValid)
            {
                if (_context.Courses.Any(c=> c.Title == course.Title && c.CourseId != course.CourseId))
                {
                    ModelState.AddModelError("Title", "A course with this title already exists");
                }
                // Prevents illogical prerequisit dependency loop
                if (course.PrerequisiteCourseId != null)
                {
                    var prerequisite = await _context.Courses
                        .FindAsync(course.PrerequisiteCourseId);

                    if (prerequisite != null &&
                        prerequisite.PrerequisiteCourseId == course.CourseId)
                    {
                        ModelState.AddModelError(
                            "PrerequisiteCourseId",
                            "Circular prerequisites are not allowed."
                        );
                    }
                }

                // checks the validity again after checking the prerequisit loop
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(course);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CourseExists(course.CourseId))
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
            }

            // repopulates the dropdown boxes incase of error
            var prerequisites = _context.Courses
                .Where(c => c.CourseId != course.CourseId)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.Title
                })
                .ToList();

            prerequisites.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "None"
            });

            ViewData["PrerequisiteCourseId"] =
                new SelectList(prerequisites, "Value", "Text", course.PrerequisiteCourseId);

            ViewData["SubjectId"] =
                new SelectList(_context.Subjects, "SubjectId", "Name", course.SubjectId);

            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.PrerequisiteCourse)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
